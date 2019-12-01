//-----------------------------------------------------------------------
// <copyright file="FilterBox.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2015. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.FilterBox
{
    /// <summary>
    /// The <see cref="FilterBox"/> class provides the code behind the resource dictionary. 
    /// </summary>
    partial class FilterBox
    {
        private static int counter;
        private static Delegate getter;
        private static Delegate setter;

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            var xamlFilterBox = textBox.Tag as XamlFilterBox;
            if (xamlFilterBox == null)
            {
                return;
            }

            var items = xamlFilterBox.ItemsSource;
            if (items == null)
            {
                return;
            }

            var filterFieldName = xamlFilterBox.FilterFieldName;
            if(string.IsNullOrWhiteSpace(filterFieldName))
            {
                return;
            }

            var visibilityFieldName = xamlFilterBox.VisibilityFieldName;
            if(string.IsNullOrWhiteSpace(visibilityFieldName))
            {
                return;
            }

            var text = textBox.Text;

            foreach (var item in items)
            {
                if (getter == null
                    && setter == null)
                {
                    var t = item.GetType();
                    CreateTypeHelpers(t, filterFieldName, visibilityFieldName);
                }

                if (string.IsNullOrEmpty(text))
                {
                    setter.DynamicInvoke(item, true);
                    continue;
                }

                var val = getter.DynamicInvoke(item);
                if (val != null
                    && val.ToString().ToLower().Contains(text.ToLower()))
                {
                    setter.DynamicInvoke(item, true);                   
                }
                else
                {
                    setter.DynamicInvoke(item, false);
                }
            }
        }
        
        public static void CreateTypeHelpers(Type t, string getterName, string setterName)
        {
            var propertyInfos = t.GetProperties();
            var getterPropertyInfo = propertyInfos.First(p => p.Name.Equals(getterName));
            var setterPropertyInfo = propertyInfos.First(p => p.Name.Equals(setterName));
            getter = GetValue(t, getterPropertyInfo);
            setter = SetValue(t, setterPropertyInfo);
        }

        private static Delegate GetValue(Type t, PropertyInfo propertyInfo)
        {
            var getAccessor = propertyInfo.GetGetMethod();
            var methodName = "GetValue_" + propertyInfo.Name + "_" + GetNextCounterValue();
            var dynMethod = new DynamicMethod(methodName, t, new Type[] { typeof(object) }, typeof(FilterBox).Module);

            ILGenerator il = dynMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, getAccessor, null);
            if (propertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            var dataType = new Type[] { t, typeof(object) };
            var genericBase = typeof(Func<,>);
            var combinedType = genericBase.MakeGenericType(dataType);

            return dynMethod.CreateDelegate(combinedType);
        }

        private static Delegate SetValue(Type t, PropertyInfo propertyInfo)
        {
            var setAccessor = propertyInfo.GetSetMethod();
            var methodName = "SetValue_" + propertyInfo.Name + "_" + GetNextCounterValue();
            var dynMethod = new DynamicMethod(methodName, typeof(void), new Type[] { t, typeof(object) }, typeof(FilterBox).Module);
            ILGenerator il = dynMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            if (propertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
            }

            il.EmitCall(OpCodes.Callvirt, setAccessor, null);
            il.Emit(OpCodes.Ret);

            var dataType = new Type[] { t, typeof(object) };
            var genericBase = typeof(Action<,>);
            var combinedType = genericBase.MakeGenericType(dataType);

            return dynMethod.CreateDelegate(combinedType);
        }

        private static int GetNextCounterValue()
        {
            return Interlocked.Increment(ref counter);
        }
    }
}