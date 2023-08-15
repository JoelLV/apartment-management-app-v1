using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Apt_Management_App.Repository
{
    internal class RenterDTO : BaseDTO, IEditableObject
    {
        private string _id = "";
        private string _name = "";
        private string _PrevName = "";
        private string _PreviousId = "";
        private string _phoneNum = "";
        private string _PrevPhoneNum = "";
        private string _email = "";
        private string _PrevEmail = "";
        public string RenterID
        {
            get { return _id; }
            set { 
                _id = value;
                NotifyPropertyChanged();
            }
        }
        public string Name
        {
            get { return _name; }
            set { 
                _name = value;
                NotifyPropertyChanged();
            }
        }
        public string PhoneNumber
        {
            get { return _phoneNum; }
            set { 
                _phoneNum = value;
                NotifyPropertyChanged();
            }
        }
        public string Email
        {
            get { return _email; }
            set { 
                _email = value;
                NotifyPropertyChanged();
            }
        }
        private bool IsInDatabase()
        /*
         * Determines whether a row
         * is in the database using
         * the renter id of the RenterDTO.
        */
        {
            var resultQuery = (from renter in _dbContext.Renters
                               where renter.RenterId == _PreviousId
                               select renter).FirstOrDefault();
            return resultQuery != null;
        }
        private Database.Renter GetNewRow()
        /*
         * Gets a new Payment object from
         * the database directory and assigns
         * all the information from 'this'
         * to the object.
         */
        {
            Database.Renter newRow = new Database.Renter();
            newRow.RenterId = RenterID;
            newRow.Name = Name;
            newRow.Phone = PhoneNumber;
            newRow.Email = Email;

            return newRow;
        }
        private void AddToDatabase()
        /*
         * Adds a new row to the database.
         */
        {
            Database.Renter rowToAdd = GetNewRow();
            _dbContext.Add(rowToAdd);
            _dbContext.SaveChanges();
        }
        private void EditToDatabase()
        /*
         * Edits an existing row
         * and commits changes to the database.
         */
        {
            var rowToUpdate = (from renter in _dbContext.Renters
                               where renter.RenterId == _PreviousId
                               select renter).FirstOrDefault();
            if (rowToUpdate != null)
            {
                rowToUpdate.Name = Name;
                rowToUpdate.Phone = PhoneNumber;
                rowToUpdate.Email = Email;
            }
            _dbContext.SaveChanges();
        }
        public static void DeleteRow(RenterDTO rowToDelete)
        /*
         * Deletes a row
         * from the database.
         * Asks for confirmation.
         */
        {
            var userInput = rowToDelete.ShowWarningMessage("Deleting a renter row will delete all references to it.\nWould you like to continue?");
            if (userInput == System.Windows.MessageBoxResult.Yes)
            {
                Database.Renter renterRow = new Database.Renter();
                renterRow.RenterId = rowToDelete.RenterID;
                renterRow.Name = rowToDelete.Name;
                renterRow.Phone = rowToDelete.PhoneNumber;
                renterRow.Email = rowToDelete.Email;

                _dbContext.Remove(renterRow);
                _dbContext.SaveChanges();
                _dbContext.ChangeTracker.Clear();
            }
        }
        public static BindingList<RenterDTO> GetRenterQuery()
        /*
         * Returns a collection of RenterDTO objects
         * to display in the datagrid.
         */
        {
            var resultQuery = (from renter in _dbContext.Renters
                               select new RenterDTO
                               {
                                   RenterID = renter.RenterId,
                                   Name = renter.Name,
                                   PhoneNumber = renter.Phone ?? "",
                                   Email = renter.Email ?? ""
                               }).ToList();
            return new BindingList<RenterDTO>(resultQuery);
        }
        public static ValidationResult ValidInput(RenterDTO input)
        /*
         * Determines whether an user
         * entered a valid input. Gets
         * called by the validation class.
         * If an error is found, it displays an error message.
         * If no error is found, it returns ValidResult.
         */
        {
            if (input.IdChanged() || input.IdAlreadyExists())
            {
                input.ShowErrorMessage("Cannot edit ids or add new rows that have an existing id.");
                return new ValidationResult(false, "");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
        public bool IdChanged()
        /*
         * Determines whether the renter id
         * changed.
         */
        {
            return _NewRow ? false : _PreviousId != RenterID;
        }
        public bool IdAlreadyExists()
        /*
         * Checks whether a row
         * exists in the database
         * using renter id of the DTO.
         */
        {
            var queryResult = (from renter in _dbContext.Renters
                               where renter.RenterId == RenterID
                               select renter).FirstOrDefault();
            return _NewRow ? queryResult != null : false;
        }
        public void BeginEdit()
        /*
         * Gets called automatically
         * by the datagrid whenever a user
         * double clicks a cell. This method
         * copies the current data in separate fields
         * in case the edit gets canceled.
         */
        {
            _EditReady = true;
            _PreviousId = RenterID;
            _PrevName = Name;
            _PrevPhoneNum = PhoneNumber;
            _PrevEmail = Email;
            if (_NewRow)
            {
                _NewRow = !IsInDatabase();
            }
        }
        public void CancelEdit()
        /*
         * Gets called automatically
         * by the datagrid whenever a user
         * presses the esc keyword button.
         * This method restores the
         * previous data that was saved
         * before the user started editing.
        */
        {
            RenterID = _PreviousId;
            Name = _PrevName;
            PhoneNumber = _PrevPhoneNum;
            Email = _PrevEmail;
        }
        public void EndEdit()
        /* 
         * Gets called automatically
         * by the datagrid whenever a user
         * presses the enter key. This method
         * commits the changes made in
         * the row to the database.
         */
        {
            if (_EditReady)
            {
                if (_NewRow)
                {
                    AddToDatabase();
                }
                else
                {
                    EditToDatabase();
                }
            }
            _EditReady = false;
            _dbContext.ChangeTracker.Clear();
        }
    }
}
