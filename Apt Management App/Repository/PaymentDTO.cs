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
    internal class PaymentDTO : BaseDTO, IEditableObject
    {
        private string _RenterName = "";
        private string _PrevName = "";
        private string _RenterId = "";
        private string _PrevRenterId = "";
        private string _ContractId = "";
        private string _PrevContractId = "";
        private string _PreviousId = "";
        private float _AmountPayed = 0;
        private float _PrevAmountPayed = 0;
        private string _DatePayed = "";
        private string _PrevDatePayed = "";
        private string _PayId = "";
        public string PaymentID
        {
            get { return _PayId; }
            set { 
                _PayId = value;
                NotifyPropertyChanged();
            }
        }
        public string ContractID
        {
            get { return _ContractId; }
            set { 
                _ContractId = value;
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
        public float AmountPayed
        {
            get { return _AmountPayed; }
            set { 
                _AmountPayed = value;
                NotifyPropertyChanged();
            }
        }
        public string DatePayed
        {
            get { return _DatePayed; }
            set { 
                _DatePayed = value;
                NotifyPropertyChanged();
            }
        }
        private Database.Payment GetPayObj()
        /*
         * Gets a new Payment object from
         * the database directory and assigns
         * all the information from 'this'
         * to the object.
         */
        {
            Database.Payment payObj = new Database.Payment();
            Database.Contract? contractObj = (from cnt in _dbContext.Contracts
                                              where cnt.ContractId == ContractID
                                              select cnt).FirstOrDefault();
            if(contractObj != null)
            {
                payObj.AmountPayed = _AmountPayed;
                payObj.DatePayed = _DatePayed;
                payObj.ContractId = ContractID;
                payObj.PayId = PaymentID;
                payObj.Contract = contractObj;
            }
            return payObj;
        }
        private void AddToDatabase()
        /*
         * Adds a new row to the database.
         */
        {
            var rowToAdd = GetPayObj();
            _dbContext.Add(rowToAdd);
            _dbContext.SaveChanges();
        }
        private void EditToDatabase()
        /*
         * Edits an existing row
         * and commits changes to the database.
         */
        {
            var queryResult = (from pay in _dbContext.Payments
                               where pay.PayId == PaymentID
                               select pay).FirstOrDefault();
            var cntResult = (from cnt in _dbContext.Contracts
                             where cnt.ContractId == ContractID
                             select cnt).FirstOrDefault();

            if(queryResult != null && cntResult != null)
            {
                queryResult.ContractId = cntResult.ContractId;
                queryResult.AmountPayed = AmountPayed;
                queryResult.DatePayed = DatePayed;
                queryResult.ContractId = ContractID;
            }
            _dbContext.SaveChanges();

        }
        private bool IsInDatabase()
        /*
         * Determines whether a row
         * is in the database using
         * the payment id of the PaymentDTO.
        */
        {
            var resultQuery = (from pay in _dbContext.Payments
                               where pay.PayId == _PreviousId
                               select pay).FirstOrDefault();
            return resultQuery != null;
        }
        private bool PayIdChanged()
        /*
         * Determines whether the payment id
         * changed.
         */
        {
            return _NewRow ? false : _PreviousId != PaymentID;
        }
        private bool IdAlreadyExists()
        /*
         * Checks whether a row
         * exists in the database
         * using payment id of the DTO.
         */
        {
            var queryResult = (from pay in _dbContext.Payments
                               where pay.PayId == PaymentID
                               select pay).FirstOrDefault();
            return _NewRow ? queryResult != null : false;
        }
        private bool HasNoReferenceIntegrity()
        /*
         * Checks for reference integrity with
         * contract.
         */
        {
            Database.Contract? contractQuery = (from contract in _dbContext.Contracts
                                               where contract.ContractId == ContractID
                                               select contract).FirstOrDefault();
            Database.Renter? renterQuery = (from renter in _dbContext.Renters
                                            where renter.RenterId == RenterID
                                            select renter).FirstOrDefault();
            if (renterQuery == null || contractQuery == null)
            {
                return true;
            }
            else
            {
                if(RenterName != renterQuery.Name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static void DeleteRow(PaymentDTO rowToDelete)
        /*
         * Deletes a row
         * from the database.
         */
        {
            var objToRemove = (from pay in _dbContext.Payments
                               where pay.PayId == rowToDelete.PaymentID
                               select pay).FirstOrDefault();
            if (objToRemove != null)
            {
                _dbContext.Remove(objToRemove);
                _dbContext.SaveChanges();
                _dbContext.ChangeTracker.Clear();
            }
        }
        public static BindingList<PaymentDTO> GetPaymentQuery()
        /*
         * Returns a collection of PaymentDTO objects
         * to display in the datagrid.
         */
        {
            var resultQuery = (from payment in _dbContext.Payments
                               join contract in _dbContext.Contracts
                               on payment.ContractId equals contract.ContractId
                               join renter in _dbContext.Renters
                               on contract.RenterId equals renter.RenterId
                               select new PaymentDTO
                               {
                                   PaymentID = payment.PayId,
                                   ContractID = contract.ContractId,
                                   RenterID = renter.RenterId,
                                   RenterName = renter.Name,
                                   AmountPayed = payment.AmountPayed,
                                   DatePayed = payment.DatePayed
                               }).ToList();
            return new BindingList<PaymentDTO>(resultQuery);
        }
        public static ValidationResult ValidInput(PaymentDTO input)
        /*
         * Determines whether an user
         * entered a valid input. Gets
         * called by the validation class.
         * If an error is found, it displays an error message.
         * If no error is found, it returns ValidResult.
         */
        {
            if (input.PayIdChanged() || input.IdAlreadyExists())
            {
                input.ShowErrorMessage("Payment id cannot be edited nor a new row can have an id that already exists.");
                return new ValidationResult(false, "");
            }
            else if (input.HasNoReferenceIntegrity())
            {
                input.ShowErrorMessage("Contract information and renter information must exist in the database.");
                return new ValidationResult(false, "");
            }
            else if (!input.ValidDates(input.DatePayed) || input.AmountPayed < 0)
            {
                input.ShowErrorMessage("Values entered are incorrect. Make sure that dates are in YYYY-MM-DD format.");
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
            _PreviousId = PaymentID;
            _PrevName = RenterName;
            _PrevRenterId = RenterID;
            _PrevContractId = ContractID;
            _PrevAmountPayed = AmountPayed;
            _PrevDatePayed = DatePayed;
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
            PaymentID = _PreviousId;
            ContractID = _PrevContractId;
            RenterID = _PrevRenterId;
            RenterName = _PrevName;
            AmountPayed = _PrevAmountPayed;
            DatePayed = _PrevDatePayed;
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
