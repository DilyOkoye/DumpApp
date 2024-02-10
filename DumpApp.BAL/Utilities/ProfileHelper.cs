using System;
using System.Web.Profile;

namespace EmailNotification.BAL.Utilities
{
    public class ProfileHelper
    {

        public void SetProfile(string userName, string propName, string propValue, string type)
        {
            int propVal = 0;
            decimal propValdecimal = 0;
            double propValdouble = 0;
            DateTime propDate = (DateTime)System.Data.SqlTypes.SqlDateTime.Null; ;
            ProfileBase profile = ProfileBase.Create(userName);
            if (type == "int")
            {
                int.TryParse(propValue, out propVal);
                profile[propName] = propVal;
            }
            else if (type == "string")
            {
                profile[propName] = propValue;
            }
            else if (type == "date")
            {
                DateTime.TryParse(propValue, out propDate);
                profile[propName] = propDate;
            }
            else if (type == "decimal")
            {
                Decimal.TryParse(propValue, out propValdecimal);
                profile[propName] = propValdecimal;
            }
            else if (type == "double")
            {
                Double.TryParse(propValue, out propValdouble);
                profile[propName] = propValdouble;
            }

            profile.Save();

            // return 
        }

        public object GetProfile(string userName, string propName)
        {
            ProfileBase profile = ProfileBase.Create(userName);

            var counter = profile[propName];


            return counter;
        }

    }

    public class profileObj
    {

        public string userName { get; set; }
        public string propName { get; set; }
        public string propValue { get; set; }
    }
}
