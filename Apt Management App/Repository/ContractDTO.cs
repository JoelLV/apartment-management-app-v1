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
    internal class ContractDTO : BaseDTO, IEditableObject
    {
        private string _ContractId = "";
        private string _PreviousId = "";
        private string _ApartmentNum = "";
        private string _PrevNum = "";
        private string _RenterName = "";
        private string _PrevName = "";
        private string _RenterId = "";
        private string _PrevRenterId = "";
        private string _ContractExpirationDate = "";
        private string _PrevContractExpDate = "";
        private byte _PaymentDay = 0;
        private byte _PrevPayDay = 0;
        private string _DateSigned = "";
        private string _PrevDateSigned = "";
        private float _DepositAmount = 0;
        private float _PrevDepoAmount = 0;
        private byte _NumOfResidens = 0;
        private byte _PrevNumRes = 0;

        public string ContractID
        {
            get { return _ContractId; }
            set { 
                _ContractId = value;
                NotifyPropertyChanged();
            }
        }
        public string ApartmentNumber
        {
            get { return _ApartmentNum; }
            set { 
                _ApartmentNum = value;
                NotifyPropertyChanged();
            }
        }
        public string RenterID
        {
            get { return _RenterId; }
            set { 
                _RenterId = value;
                NotifyPropertyChanged();
            }
        }
        public string RenterName
        {
            get { return _RenterName; }
            set { 
                _RenterName = value;
                NotifyPropertyChanged();
            }
        }
        public string ContractExpirationDate
        {
            get { return _ContractExpirationDate; }
            set { 
                _ContractExpirationDate = value;
                NotifyPropertyChanged();
            }
        }
        public byte PaymentDay
        {
            get { return _PaymentDay; }
            set { 
                _PaymentDay = value;
                NotifyPropertyChanged();
            }
        }
        public string DateSigned
        {
            get { return _DateSigned; }
            set { 
                _DateSigned = value;
                NotifyPropertyChanged();
            }
        }
        public float DepositAmount
        {
            get { return _DepositAmount; }
            set { 
                _DepositAmount = value;
                NotifyPropertyChanged();
            }
        }
        public byte NumberOfResidents
        {
            get { return _NumOfResidens; }
            set { 
                _NumOfResidens = value;
                NotifyPropertyChanged();
            }
        }
        private Database.Contract GetContractObj()
        /*
         * Gets a new Contract object from
         * the database directory and assigns
         * all the information from 'this'
         * to the object.
         */
        {
            Database.Contract contract = new Database.Contract();
            contract.Apt = (from apt in _dbContext.Apartments
                           where ApartmentNumber == apt.AptNum
                           select apt).FirstOrDefault();
            contract.Renter = (from rnt in _dbContext.Renters
                               where rnt.RenterId == RenterID
                               select rnt).FirstOrDefault();

            if (contract.Renter != null && contract.Apt != null)
            {
                contract.ContractId = ContractID;
                contract.ExpDate = ContractExpirationDate;
                contract.PaymentDay = PaymentDay;
                contract.DateSigned = DateSigned;
                contract.Deposit = DepositAmount;
                contract.NumResidents = NumberOfResidents;
                contract.Apt.AptNum = ApartmentNumber;
                contract.Renter.Name = RenterName;
            }
            return contract;
        }
        private void AddToDatabase()
        /*
         * Adds a new row to the database.
         */
        {
            var rowToAdd = GetContractObj();
            _dbContext.Add(rowToAdd);
            _dbContext.SaveChanges();
        }
        private void EditToDatabase()
        /*
         * Edits an existing row
         * and commits changes to the database.
         */
        {
            var resultQuery = (from cnt in _dbContext.Contracts
                              where cnt.ContractId == ContractID
                              select cnt).FirstOrDefault();
            var aptQuery = (from apt in _dbContext.Apartments
                            where apt.AptNum == ApartmentNumber
                            select apt).FirstOrDefault();
            var rntQuery = (from rnt in _dbContext.Renters
                            where rnt.RenterId == RenterID
                            select rnt).FirstOrDefault();

            if (resultQuery != null && aptQuery != null && rntQuery != null)
            {
                resultQuery.AptId = aptQuery.AptId;
                resultQuery.RenterId = rntQuery.RenterId;
                resultQuery.DateSigned = DateSigned;
                resultQuery.Deposit = DepositAmount;
                resultQuery.NumResidents = NumberOfResidents;
                resultQuery.ExpDate = ContractExpirationDate;
                resultQuery.PaymentDay = PaymentDay;
            }
            _dbContext.SaveChanges();
        }
        public static ValidationResult ValidInput(ContractDTO input)
        /*
         * Determines whether an user
         * entered a valid input. Gets
         * called by the validation class.
         * If an error is found, it displays an error message.
         * If no error is found, it returns ValidResult.
         */
        {
            if (input.ContractIdChanged() || input.IdAlreadyExists())
            {
                input.ShowErrorMessage("Cannot edit contract id or add an id that already exists.");
                return new ValidationResult(false, "");
            }
            else if(input.HasNoReferenceIntegrity())
            {
                input.ShowErrorMessage("The apartment number or name and id of renter does not exist in database.");
                return new ValidationResult(false, "");
            }
            else if (!input.ValidDates(input.ContractExpirationDate, input.DateSigned) || input.PaymentDay < 0)
            {
                input.ShowErrorMessage("Dates entered are invalid. Check that the date is in YYYY-MM-DD format.");
                return new ValidationResult(false, "");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
        public bool IdAlreadyExists()
        /*
         * Checks whether a row
         * exists in the database
         * using ContractID of the DTO.
         */
        {
            var queryResult = (from cnt in _dbContext.Contracts
                               where cnt.ContractId == ContractID
                               select cnt).FirstOrDefault();
            return _NewRow ? queryResult != null : false;
        }
        public bool ContractIdChanged()
        /*
         * Determines whether the contract id
         * changed.
         */
        {
            return _NewRow ? false : _PreviousId != ContractID;
        }
        public bool HasNoReferenceIntegrity()
        /*
         * Checks for reference integrity between
         * renter and apartment.
         */
        {
            Database.Renter? renterQuery = (from renter in _dbContext.Renters
                                            where renter.RenterId == RenterID
                                            select renter).FirstOrDefault();
            Database.Apartment? aptQuery = (from apt in _dbContext.Apartments
                                            where apt.AptNum == ApartmentNumber
                                            select apt).FirstOrDefault();

            if(renterQuery == null || aptQuery == null)
            {
                return true;
            }
            else if(renterQuery.Name != RenterName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void DeleteRow(ContractDTO contractObj)
        /*
         * Prompts user to confirm
         * whether he wants to delete
         * the row or not due to delete
         * cascading. If user accepts,
         * the row is deleted from
         * the database.
         */
        {
            var userInput = contractObj.ShowWarningMessage("Deleting a contract will delete all references to it, Would you like to continue?");
            if (userInput == System.Windows.MessageBoxResult.Yes)
            {
                var resultQuery = (from cnt in _dbContext.Contracts
                                   where cnt.ContractId == contractObj.ContractID
                                   select cnt).FirstOrDefault();
                if (resultQuery != null)
                {
                    _dbContext.Remove(resultQuery);
                    _dbContext.SaveChanges();
                    _dbContext.ChangeTracker.Clear();
                }
            }
        }
        public static BindingList<ContractDTO> GetContractQuery()
        /*
         * Returns a collection of ContractDTO objects
         * to display in the datagrid.
         */
        {
            var resultQuery = (from contract in _dbContext.Contracts
                               join renter in _dbContext.Renters
                               on contract.RenterId equals renter.RenterId
                               join apt in _dbContext.Apartments
                               on contract.AptId equals apt.AptId
                               select new ContractDTO
                               {
                                   ContractID = contract.ContractId,
                                   ApartmentNumber = apt.AptNum,
                                   RenterID = renter.RenterId,
                                   RenterName = renter.Name,
                                   ContractExpirationDate = contract.ExpDate,
                                   PaymentDay = contract.PaymentDay,
                                   DateSigned = contract.DateSigned,
                                   DepositAmount = contract.Deposit,
                                   NumberOfResidents = contract.NumResidents
                               }).ToList();
            return new BindingList<ContractDTO>(resultQuery);
        }
        public bool IsInDatabase()
        /*
         * Determines whether a row
         * is in the database using
         * the contract id of the ContractDTO.
         */
        {
            var resultQuery = (from cnt in _dbContext.Contracts
                              where cnt.ContractId == ContractID
                              select cnt).FirstOrDefault();
            return resultQuery != null;
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
            _PrevNum = ApartmentNumber;
            _PrevRenterId = RenterID;
            _PrevName = RenterName;
            _PrevContractExpDate = ContractExpirationDate;
            _PrevPayDay = PaymentDay;
            _PrevDateSigned = DateSigned;
            _PrevDepoAmount = DepositAmount;
            _PrevNumRes = NumberOfResidents;

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
            ApartmentNumber = _PrevNum;
            RenterID = _PrevRenterId;
            RenterName = _PrevName;
            ContractExpirationDate = _PrevContractExpDate;
            PaymentDay = _PrevPayDay;
            DateSigned = _PrevDateSigned;
            DepositAmount = _PrevDepoAmount;
            NumberOfResidents = _PrevNumRes;
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
