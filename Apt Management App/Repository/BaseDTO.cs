using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Apt_Management_App.Repository
{
    internal class BaseDTO : INotifyPropertyChanged
    {
        protected static Database.ApartmentDbContext _dbContext = new Database.ApartmentDbContext();
        protected bool _EditReady = false;
        protected bool _NewRow = true;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        /*
         * This method is in charge of notifying the view
         * that a given property has changed.
        */
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool IsLeapYear(int year)
        /*
         * Determines whether
         * a year is leap year
         * or not.
         */
        {
            if (year % 4 == 0 && year % 100 != 0)
            {
                return true;
            }
            else if (year % 400 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ValidNums(string[] brokenDate)
        /*
         * Determines whether the numbers in
         * the date can be converted to an int.
         */
        {
            for (int i = 0; i < brokenDate.Length; i++)
            {
                if (!int.TryParse(brokenDate[i], out int result1))
                {
                    return false;
                }
            }
            return true;
        }
        private bool ValidDate(string[] date)
        /*
         * Determines whether the
         * given date is a valid date.
         * The parameter is a string array
         * with the first element being the year,
         * the second element is the month,
         * and the third element is the day.
         */
        {
            Dictionary<int, int> monthMap = new Dictionary<int, int>()
            {
                {1, 31},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10,31},
                {11,30},
                {12,31}
            };
            int year = int.Parse(date[0]);
            int month = int.Parse(date[1]);
            int day = int.Parse(date[2]);

            if (month == 2)
            {
                if (IsLeapYear(year))
                {
                    return day <= 29;
                }
                else
                {
                    return day <= 28;
                }
            }
            else
            {
                if (!monthMap.ContainsKey(month))
                {
                    return false;
                }
                else
                {
                    if (monthMap[month] == 31)
                    {
                        return day <= 31;
                    }
                    else
                    {
                        return day <= 30;
                    }
                }
            }
        }
        protected bool ValidFormat(string date)
        /*
         * Determines whether the given
         * string is in the desirable format.
         * This means that a '-' must separate
         * the day and the year from month, the numbers
         * must be actual numbers, and the date must be
         * a valid date.
         */
        {
            if (!date.Contains('-'))
            {
                return false;
            }

            string[] brokenDate = date.Split('-');
            if (brokenDate.Length != 3)
            {
                return false;
            }
            else if (!ValidNums(brokenDate))
            {
                return false;
            }
            else
            {
                if (!ValidDate(brokenDate))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        protected bool ValidDates(params string[] dates)
        /*
         * This method accepts 1 or more string
         * representing different dates, and checks
         * whether every date passed is valid, otherwise
         * it returns false.
         */
        {
            for(int i = 0; i < dates.Length; i++)
            {
                if (!ValidFormat(dates[i]))
                {
                    return false;
                }
            }
            return true;
        }
        protected MessageBoxResult ShowWarningMessage(string message)
        /*
         * Displays a warning message
         * for the user. Generally
         * used when the user wants to delete
         * something that will cascade in the database.
         */
        {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            string caption = "Delete Warning";

            return MessageBox.Show(message, caption, button, icon, MessageBoxResult.No);
        }
        protected void ShowErrorMessage(string message)
        /*
         * Displays an error message
         * for the user. Generally
         * used when the user enters
         * invalid input in the datagrid.
         */
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;
            string caption = "Invalid Input";

            MessageBox.Show(message, caption, button, icon, MessageBoxResult.OK);
        }
        
    }
}