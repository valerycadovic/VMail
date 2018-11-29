using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;

namespace VMail.Models
{
    public class User
    {
        /// <summary>
        /// The address cross name
        /// </summary>
        public const string address_cross_name = nameof(address_cross_name);

        /// <summary>
        /// The password cross name
        /// </summary>
        public const string password_cross_name = nameof(password_cross_name);

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address
        {
            get => CrossSettings.Current.GetValueOrDefault(address_cross_name, "");
            set
            {
                if(Address != value)
                {
                    CrossSettings.Current.AddOrUpdateValue(address_cross_name, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get => CrossSettings.Current.GetValueOrDefault(password_cross_name, "");
            set
            {
                if (Password != value)
                {
                    CrossSettings.Current.AddOrUpdateValue(password_cross_name, value);
                }
            }
        }
    }
}
