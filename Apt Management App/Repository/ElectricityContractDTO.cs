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
    internal class ElectricityContractDTO : BaseDTO, IEditableObject
    {
        private string _AptNum = "";
        private string _PrevAptNum = "";
        private string _PreviousId = "";
        private string _ElecId = "";
        private string _ServiceNum = "";
        private string _PrevServNum = "";
        private string _MeasurerNum = "";
        private string _PrevMeasurerNum = "";
        private string _Rmu = "";
        private string _PrevRmu = "";
        private string _PaymentDue = "";
        private string _PrevPayDue = "";
        private string _ShutOffDate = "";
        private string _PrevShutOffDate = "";
        public string ContractID
        {
            get { return _ElecId; }
            set { 
                _ElecId = value;
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
        public string ServiceNumber
        {
            get { return _ServiceNum; }
            set { 
                _ServiceNum = value;
                NotifyPropertyChanged();
            }
        }
        public string MeasurerNumber
        {
            get { return _MeasurerNum; }
            set { 
                _MeasurerNum= value;
                NotifyPropertyChanged();
            }
        }
        public string RMU
        {
            get { return _Rmu; }
            set { 
                _Rmu = value;
                NotifyPropertyChanged();
            }
        }
        public string PaymentDue
        {
            get { return _PaymentDue; }
            set {
                _PaymentDue = value;
                NotifyPropertyChanged();
            }
        }
        public string ShutOffDate
        {
            get { return _ShutOffDate; }
            set { 
                _ShutOffDate = value;
                NotifyPropertyChanged();
            }
        }
        private bool IsInDatabase()
        /*
         * Determines whether a row
         * is in the database using
         * the contract id of the ElectricityContractDTO.
        */
        {
            var resultQuery = (from elec in _dbContext.ElectricityContracts
                               where elec.ElecContractId == ContractID
                               select elec).FirstOrDefault();
            return resultQuery != null;
        }
        private Database.ElectricityContract GetElecContractObj()
        /*
         * Gets a new Contract object from
         * the database directory and assigns
         * all the information from 'this'
         * to the object.
         */
        {
            var aptQuery = (from apt in _dbContext.Apartments
                            where apt.AptNum == ApartmentNumber
                            select apt).FirstOrDefault();

            Database.ElectricityContract newRow = new Database.ElectricityContract();
            if (aptQuery != null)
            {
                newRow.Apt = aptQuery;
                newRow.ElecContractId = ContractID;
                newRow.ServiceNum = ServiceNumber;
                newRow.MeasurerNum = MeasurerNumber;
                newRow.Rmu = RMU;
                newRow.PaymentDue = PaymentDue;
                newRow.ShutOffDate = ShutOffDate;
            }
            return newRow;
        }
        private void AddToDatabase()
        /*
         * Adds a new row to the database.
         */
        {
            Database.ElectricityContract rowToAdd = GetElecContractObj();
            _dbContext.Add(rowToAdd);
            _dbContext.SaveChanges();
        }
        private void EditToDatabase()
        /*
         * Edits an existing row
         * and commits changes to the database.
         */
        {
            var resultQuery = (from elec in _dbContext.ElectricityContracts
                               where elec.ElecContractId == _PreviousId
                               select elec).FirstOrDefault();
            var aptQuery = (from apt in _dbContext.Apartments
                            where apt.AptNum == ApartmentNumber
                            select apt).FirstOrDefault();

            if(resultQuery != null && aptQuery != null)
            {
                resultQuery.AptId = aptQuery.AptId;
                resultQuery.ElecContractId = ContractID;
                resultQuery.ServiceNum = ServiceNumber;
                resultQuery.MeasurerNum = MeasurerNumber;
                resultQuery.Rmu = RMU;
                resultQuery.PaymentDue = PaymentDue;
                resultQuery.ShutOffDate = ShutOffDate;
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
         * using ContractID of the DTO.
         */
        {
            var queryResult = (from elec in _dbContext.ElectricityContracts
                               where elec.ElecContractId == ContractID
                               select elec).FirstOrDefault();
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
        public static BindingList<ElectricityContractDTO> GetElectricityContractQuery()
        /*
         * Returns a collection of ElectricityContractDTO objects
         * to display in the datagrid.
         */
        {
            var resultQuery = (from elecContract in _dbContext.ElectricityContracts
                               join apt in _dbContext.Apartments
                               on elecContract.AptId equals apt.AptId
                               select new ElectricityContractDTO
                               {
                                   ContractID = elecContract.ElecContractId,
                                   ApartmentNumber = apt.AptNum,
                                   ServiceNumber = elecContract.ServiceNum,
                                   MeasurerNumber = elecContract.MeasurerNum,
                                   RMU = elecContract.Rmu,
                                   PaymentDue = elecContract.PaymentDue,
                                   ShutOffDate = elecContract.ShutOffDate
                               }).ToList();
            return new BindingList<ElectricityContractDTO>(resultQuery);
        }
        public static void DeleteRow(ElectricityContractDTO rowToDelete)
        /*
         * Deletes a row
         * from the database.
         */
        {
            Database.ElectricityContract? electricityObj = (from elec in _dbContext.ElectricityContracts
                                                            where elec.ElecContractId == rowToDelete.ContractID
                                                            select elec).FirstOrDefault();
            if (electricityObj != null)
            {
                _dbContext.Remove(electricityObj);
                _dbContext.SaveChanges();
                _dbContext.ChangeTracker.Clear();
            }
        }
        public static ValidationResult ValidInput(ElectricityContractDTO input)
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
                input.ShowErrorMessage("Cannot edit Contract id or add new row that has the same contract id as another row.");
                return new ValidationResult(false, "");
            }
            else if (input.HasNoReferentialIntegrity())
            {
                input.ShowErrorMessage("Apartment information entered does not match database.");
                return new ValidationResult(false, "");
            }
            else if (!input.ValidDates(input.PaymentDue, input.ShutOffDate))
            {
                input.ShowErrorMessage("Invalid dates entered. Make sure that the dates are entered in YYYY-MM-DD format.");
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
            _PrevAptNum = ApartmentNumber;
            _PrevServNum = ServiceNumber;
            _PrevMeasurerNum = MeasurerNumber;
            _PrevRmu = RMU;
            _PrevPayDue = PaymentDue;
            _PrevShutOffDate = ShutOffDate;

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
            ApartmentNumber = _PrevAptNum;
            ServiceNumber = _PrevServNum;
            MeasurerNumber = _PrevMeasurerNum;
            RMU = _PrevRmu;
            PaymentDue = _PrevPayDue;
            ShutOffDate = _PrevShutOffDate;
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
            _NewRow = false;
            _EditReady = false;
            _dbContext.ChangeTracker.Clear();
        }
    }
}
