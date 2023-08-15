using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Apt_Management_App.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _TableNameSelected = "";
        private object? _DataToDisplay = null;
        private object? _SelectedRow = null;

        public event PropertyChangedEventHandler? PropertyChanged;
        public DelegateCommand DeleteRowCommand { get; set; }
        public object? SelectedRow
        {
            get { return _SelectedRow; }
            set { _SelectedRow = value; }
        }
        public List<string> TableNames
        {
            get { return GetTableNames(); }
        }
        public string TableNameSelected
        {
            get { return _TableNameSelected; }
            set
            {
                _TableNameSelected = value;
                DataToDisplay = GetDataToDisplay();
            }
        }
        public object? DataToDisplay
        {
            get { return _DataToDisplay; }
            set
            {
                _DataToDisplay = value;
                NotifyPropertyChanged();
            }
        }
        public MainViewModel()
        {
            DeleteRowCommand = new DelegateCommand(DeleteRow, (object o) => SelectedRow != null);
        }
        private List<string> GetTableNames()
        /*
         * Calls getTableNames method in GeneralDatabaseInfo to
         * get all necessary table names to display in the left side
         * of the main window.
        */
        {
            return Repository.GeneralDatabaseInfo.GetTableNames();
        }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        /*
         * This method is in charge of notifying the view
         * that a given property has changed.
        */
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private object GetDataToDisplay()
        /*
         * Gets appropriate BindingList according to the
         * current TableNameSelected to display in the datagrid.
         */
        {
            switch (TableNameSelected)
            {
                case "Apartments":
                    return GetApartmentInfo();
                case "Renters":
                    return GetRenterInfo();
                case "Contracts":
                    return GetContractInfo();
                case "Payments":
                    return GetPaymentInfo();
                case "Water Bills":
                    return GetWaterContractInfo();
                case "Electricity Bills":
                    return GetElectricityContractInfo();
                default:
                    return GetApartmentInfo(); //Unreachable
            }
        }

        //--------------------------------------------------------------------------------
        /*
         * Helper functions that call the appropriate DTO
         * to perform a query in the database.
        */
        private BindingList<Repository.ApartmentDTO> GetApartmentInfo()
        {
            return Repository.ApartmentDTO.GetApartmentQuery();
        }
        private BindingList<Repository.RenterDTO> GetRenterInfo()
        {
            return Repository.RenterDTO.GetRenterQuery();
        }
        private BindingList<Repository.ContractDTO> GetContractInfo()
        {
            return Repository.ContractDTO.GetContractQuery();
        }
        private BindingList<Repository.PaymentDTO> GetPaymentInfo()
        {
            return Repository.PaymentDTO.GetPaymentQuery();
        }
        private BindingList<Repository.WaterContractDTO> GetWaterContractInfo()
        {
            return Repository.WaterContractDTO.GetWaterContractQuery();
        }
        private BindingList<Repository.ElectricityContractDTO> GetElectricityContractInfo()
        {
            return Repository.ElectricityContractDTO.GetElectricityContractQuery();
        }
        //---------------------------------------------------------------------------------
        public void DeleteRow(object o)
        /*
         * Calls the appropriate delete method according
         * to the current selected row in the datagrid.
         * Gets called when an user presses the delete button
         * in the keyword.
         */
        {
            switch (SelectedRow)
            {
                case Repository.ApartmentDTO:
                    Repository.ApartmentDTO apartment = (Repository.ApartmentDTO)SelectedRow;
                    Repository.ApartmentDTO.DeleteRow(apartment);
                    DataToDisplay = GetApartmentInfo();
                    break;
                case Repository.ContractDTO:
                    Repository.ContractDTO contract = (Repository.ContractDTO)SelectedRow;
                    Repository.ContractDTO.DeleteRow(contract);
                    DataToDisplay = GetContractInfo();
                    break;
                case Repository.ElectricityContractDTO:
                    Repository.ElectricityContractDTO elecContract = (Repository.ElectricityContractDTO)SelectedRow;
                    Repository.ElectricityContractDTO.DeleteRow(elecContract);
                    DataToDisplay = GetElectricityContractInfo();
                    break;
                case Repository.PaymentDTO:
                    Repository.PaymentDTO payment = (Repository.PaymentDTO)SelectedRow;
                    Repository.PaymentDTO.DeleteRow(payment);
                    DataToDisplay = GetPaymentInfo();
                    break;
                case Repository.RenterDTO:
                    Repository.RenterDTO renter = (Repository.RenterDTO)SelectedRow;
                    Repository.RenterDTO.DeleteRow(renter);
                    DataToDisplay = GetRenterInfo();
                    break;
                case Repository.WaterContractDTO:
                    Repository .WaterContractDTO waterContract = (Repository .WaterContractDTO)SelectedRow;
                    Repository .WaterContractDTO.DeleteRow(waterContract);
                    DataToDisplay = GetWaterContractInfo();
                    break;
            }
        }
    }
}
