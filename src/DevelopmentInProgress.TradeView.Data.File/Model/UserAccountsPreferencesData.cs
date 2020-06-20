using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Data.File.Model
{
    public class UserAccountsPreferencesData
    {
        public UserAccountsPreferencesData()
        {
            Accounts = new List<UserAccountPreferencesData>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Setter required for serializing and deserialising the object to a data file.")]
        public List<UserAccountPreferencesData> Accounts { get; set; }
    }
}