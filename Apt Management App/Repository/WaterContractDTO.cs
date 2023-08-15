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
    internal class WaterContractDTO : BaseDTO, IEditableObject
    {
        private string _AccountNum = "";
        private string _PrevAccountNum = "";
        private string _ContractId = "";
        private string _AptNum = "";
        private string _PrevNum = "";
        private string _ConsumeStartDate = "";
        private string _PrevConsumeStartDate = "";
        private string _ConsumeEndDate = "";
        private string _PrevConsumeEndDate = "";
        private string _ExpDate = "";
        private string _PrevExpDate = "";
        private string _PreviousId = "";
        public string ContractID
        {
            get { return _ContractId; }
            set { 
                _ContractId = value;
                NotifyPropertyChanged();
            }
        }
        public string AccountNumber
        {
            get { return _AccountNum; }
            set { 
                _AccountNum = value;
                NotifyPropertyChanged();
            }
        }
        public string ConsumeStartDate
        {
            get { return _ConsumeStartDate; }
            set { 
                _ConsumeStartDate = value;
                NotifyPropertyChanged();
            }
        }
        public string ConsumeEndDate
        {
            get { return _ConsumeEndDate; }
            set { 
                _ConsumeEndDate = value;
                NotifyPropertyChanged();
            }
        }
        public string ExpirationDate
        {
            get { return _ExpDate; }
            set { 
                _ExpDate = value;
                NotifyPropertyChanged();
            }
        }
        public string ApartmentNumber
        {
            get { return _AptNum; }
            set { 
                _AptNum = value;
                NotifyPropertyChanged();
            }
        }
        private bool IsInDatabase()
        /*
         * Determines whether a row
         * is in the database using
         * the contract id of the WaterContractDTO.
        */
        {
            var resultQuery = (from cnt in _dbContext.WaterContracts
                               where cnt.WaterContractId == ContractID
                               select cnt).FirstOrDefault();
            return resultQuery != null;
        }
        private Database.WaterContract GetContractObj()
        /*
         * Gets a new WaterContract object from
         * the database directory and assigns
         * all the information from 'this'
         * to the object.
         */
        {
            Database.WaterContract newObj = new Database.WaterContract();

            var apartment = (from apt in _dbContext.Apartments
                             where apt.AptNum == ApartmentNumber
                             select apt).FirstOrDefault();
            if(apartment != null)
            {
                newObj.Apt = apartment;
                newObj.WaterContractId = ContractID;
                newObj.AccountNum = AccountNumber;
                newObj.ConsumeEndDate = ConsumeEndDate;
                newObj.ExpDate = ExpirationDate;
                newObj.ConsumeStartDate = ConsumeStartDate;
            }
            return newObj;
        }
        private void AddToDatabase()
        /*
         * Adds a new row to the database.
         */
        {
            Database.WaterContract rowToAdd = GetContractObj();
            _dbContext.Add(rowToAdd);
            _dbContext.SaveChanges();
        }
        private void EditToDatabase()
        /*
         * Edits an existing row
         * and commits changes to the database.
         */
        {
            var resultQuery = (from water in _dbContext.WaterContracts
                               where water.WaterContractId == _PreviousId
                               select water).FirstOrDefault();
            var aptQuery = (from apt in _dbContext.Apartments
                            where apt.AptNum == ApartmentNumber
                            select apt).FirstOrDefault();

            if (aptQuery != null && resultQuery != null)
            {
                resultQuery.AptId = aptQuery.AptId;
                resultQuery.AccountNum = AccountNumber;
                resultQuery.ConsumeEndDate = ConsumeEndDate;
                resultQuery.ExpDate = ExpirationDate;
                resultQuery.ConsumeStartDate = ConsumeStartDate;
            }
            _dbContext.SaveChanges();
        }
        public bool IdChanged()
        /*
         * Determines whether the contract id
         * changed.
         */
        {
            return _NewRow ? false : _PreviousId != ContractID;
        }
        public bool IdAlreadyExists()
        /*
         * Checks whether a row
         * exists in the database
         * using contract id of the DTO.
         */
        {
            var queryResult = (from water in _dbContext.WaterContracts
                               where water.WaterContractId == ContractID
                               select water).FirstOrDefault();
            return _NewRow ? queryResult != null : false;
        }
        public bool HasNoReferentialIntegrity()
        /*
         * Checks for reference integrity with
         * apartment.
         */
        {
            var resultQuery = (from apt in _dbContext.Apartments
                               where apt.AptNum == ApartmentNumber
                               select apt).FirstOrDefault();
            return resultQuery == null;
        }
        public static BindingList<WaterContractDTO> GetWaterContractQuery()
        /*
         * Returns a collection of WaterContractDTO objects
         * to display in the datagrid.
         */
        {
            var resultQuery = (from waterContract in _dbContext.WaterContracts
                               join apartment in _dbContext.Apartments
                               on waterContract.AptId equals apartment.AptId
                               select new WaterContractDTO
                               {
                                   ContractID = waterContract.WaterContractId,
                                   AccountNumber = waterContract.AccountNum,
                                   ConsumeStartDate = waterContract.ConsumeStartDate,
                                   ConsumeEndDate = waterContract.ConsumeEndDate,
                                   ExpirationDate = waterContract.ExpDate,
                                   ApartmentNumber = apartment.AptNum
                               }).ToList();
            return new BindingList<WaterContractDTO>(resultQuery);
        }
        public static void DeleteRow(WaterContractDTO rowToDelete)
        /*
         * Deletes a row
         * from the database.
         */
        {
            Database.WaterContract? waterObj = (from waterContract in _dbContext.WaterContracts
                                               where waterContract.WaterContractId == rowToDelete.ContractID
                                               select waterContract).FirstOrDefault();
            if (waterObj != null)
            {
                _dbContext.Remove(waterObj);
                _dbContext.SaveChanges();
                _dbContext.ChangeTracker.Clear();
            }
        }
        public static ValidationResult ValidInput(WaterContractDTO input)
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
                input.ShowErrorMessage("Cannot change id nor add a new id that already exists.");
                return new ValidationResult(false, "");
            }
            else if (input.HasNoReferentialIntegrity())
            {
                input.ShowErrorMessage("Apartment number does not exist in database.");
                return new ValidationResult(false, "");
            }
            else if (!input.ValidDates(input.ConsumeEndDate, input.ConsumeStartDate, input.ExpirationDate))
            {
                input.ShowErrorMessage("Invalid input of dates. Make sure that dates are in YYYY-MM-DD format.");
                return new ValidationResult(false, "");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
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
            _PreviousId = ContractID;
            _PrevAccountNum = AccountNumber;
            _PrevConsumeEndDate = ConsumeEndDate;
            _PrevConsumeStartDate = ConsumeStartDate;
            _PrevExpDate = ExpirationDate;
            _PrevNum = ApartmentNumber;

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
            ContractID = _PreviousId;
            AccountNumber = _PrevAccountNum;
            ConsumeEndDate = _PrevConsumeEndDate;
            ConsumeStartDate = _PrevConsumeStartDate;
            ExpirationDate = _PrevExpDate;
            ApartmentNumber = _PrevNum;
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
            _EditReady= false;
            _NewRow = false;
            _dbContext.ChangeTracker.Clear();
        }
    }
}
