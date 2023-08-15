using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Apt_Management_App.Repository
{
    internal class ApartmentDTO : BaseDTO, IEditableObject
    {
        private string _ApartmentNum = "";
        private string _PreviousNum = "";
        private byte _Capacity = 0;
        private byte _PrevCapacity = 0;
        private float _MonthlyCost = 0;
        private float _PrevCost = 0;
        private byte _Bedrooms = 0;
        private byte _PrevBedrooms = 0;
        private bool _HasKitchen = false;
        private bool _PrevKitchen = false;
        private byte _Bathrooms = 0;
        private byte _PrevBathrooms = 0;

        public string ApartmentNumber
        {
            get { return _ApartmentNum; }
            set { 
                _ApartmentNum = value;
                NotifyPropertyChanged();
            }
        }
        public byte Capacity
        {
            get { return _Capacity; }
            set { 
                _Capacity = value;
                NotifyPropertyChanged();
            }
        }
        public float MonthlyCost
        {
            get { return _MonthlyCost; }
            set { 
                _MonthlyCost = value;
                NotifyPropertyChanged();
            }
        }
        public byte Bedrooms
        {
            get { return _Bedrooms; }
            set { 
                _Bedrooms = value;
                NotifyPropertyChanged();
            }
        }
        public bool HasKitchen
        {
            get { return _HasKitchen; }
            set { 
                _HasKitchen = value;
                NotifyPropertyChanged();
            }
        }
        public byte Bathrooms
        {
            get { return _Bathrooms; }
            set { 
                _Bathrooms = value;
                NotifyPropertyChanged();
            }
        }
        public static ValidationResult ValidInput(ApartmentDTO input)
        /*
         * Gets called from the Validation class before
         * any information is inserted in the database.
         * Displays an error window whenever an error
         * is detected.
         */
        {
            var queryResult = (from apt in _dbContext.Apartments
                              where apt.AptNum == input.ApartmentNumber
                              select apt).FirstOrDefault();
            if (queryResult == null)
            {
                return ValidationResult.ValidResult;
            }
            else if (!input.ApartmentNumChanged())
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                input.ShowErrorMessage("Apartment number already exists in databse.");
                return new ValidationResult(false, "");
            }
        }
        public bool ApartmentNumChanged()
        /*
         * Determines whether the
         * apartment number was edited
         * in the datagrid.
        */

        {
            return ApartmentNumber != _PreviousNum;
        }
        public string GetNextId()
        /*
         * Returns a string representing
         * the next apartment id to add
         * to the database.
         */
        {
            var resultQuery = (from apt in _dbContext.Apartments
                               orderby apt.AptId ascending
                               select apt.AptId).ToList();
            int currMax = 0;
            for (int i = 0; i < resultQuery.Count; i++)
            {
                int nextId = int.Parse(resultQuery[i]);
                if (nextId > currMax)
                {
                    currMax = nextId;
                }
            }
            return (currMax + 1).ToString();
        }
        private Database.Apartment GetNewApartmentObj()
        /*
         * Copies the information of 'this' and
         * transfers it into an apartment object
         * from the database.
         */
        {
            Database.Apartment newApartment = new Database.Apartment();
            newApartment.AptId = GetNextId();
            newApartment.AptNum = ApartmentNumber;
            newApartment.Capacity = Capacity;
            newApartment.MonthlyCost = MonthlyCost;
            newApartment.Bedrooms = Bedrooms;
            newApartment.HasKitchen = HasKitchen;
            newApartment.Bathrooms = Bathrooms;

            return newApartment;
        }
        private bool IsInDatabase()
        /*
         * Determines whether a
         * row is already in the database
         * by using apartment number.
         */
        {
            var resultQuery = (from apt in _dbContext.Apartments
                               where apt.AptNum == _PreviousNum
                               select apt).ToList();
            return resultQuery.Count > 0;
        }
        private void AddToDatabase()
        /*
         * Adds a new row to the database.
         */
        {
            var newRow = GetNewApartmentObj();
            _dbContext.Add(newRow);
            _dbContext.SaveChanges();
        }
        private void EditToDatabase()
        /*
         * Edits the row of the database
         * and commits changes.
         */
        {
            var resultQuery = (from apt in _dbContext.Apartments
                              where apt.AptNum == _PreviousNum
                              select apt).FirstOrDefault();

            if (resultQuery != null)
            {
                resultQuery.AptNum = ApartmentNumber;
                resultQuery.Bathrooms = Bathrooms;
                resultQuery.Bedrooms = Bedrooms;
                resultQuery.Capacity = Capacity;
                resultQuery.MonthlyCost = MonthlyCost;
                resultQuery.HasKitchen = HasKitchen;
            }
            _dbContext.SaveChanges();
        }
        public static BindingList<ApartmentDTO> GetApartmentQuery()
        /*
         * Used by the MainViewModel to get all the
         * information needed to display as a collection
         * of ApartmentDTO objects.
         */
        {
            var resultQuery = (from apt in _dbContext.Apartments
                               orderby apt.AptNum ascending
                               select new ApartmentDTO
                              {
                                  ApartmentNumber = apt.AptNum,
                                  Capacity = apt.Capacity,
                                  MonthlyCost = apt.MonthlyCost,
                                  Bedrooms = apt.Bedrooms,
                                  HasKitchen = apt.HasKitchen,
                                  Bathrooms = apt.Bathrooms
                              }).ToList();

            return new BindingList<ApartmentDTO>(resultQuery);
        }
        public static void DeleteRow(ApartmentDTO apartmentObj)
        /*
         * Deletes given row. Asks for confirmation
         * first because deletion cascades.
         */
        {
            var userConfirmation = apartmentObj.ShowWarningMessage("Deleting an apartment row will delete all rows\nin the database that reference to this specific apartment.\nWould you like to continue?");
            if (userConfirmation == MessageBoxResult.Yes)
            {
                var resultQuery = (from apt in _dbContext.Apartments
                                   where apartmentObj.ApartmentNumber == apt.AptNum
                                   select apt).FirstOrDefault();
                if(resultQuery != null)
                {
                    if (userConfirmation == MessageBoxResult.Yes)
                    {
                        _dbContext.Remove(resultQuery);
                        _dbContext.SaveChanges();
                        _dbContext.ChangeTracker.Clear();
                    }

                }
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
            _PreviousNum = ApartmentNumber;
            _PrevCapacity = Capacity;
            _PrevCost = MonthlyCost;
            _PrevBedrooms = Bedrooms;
            _PrevKitchen = HasKitchen;
            _PrevBathrooms = Bathrooms;
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
            ApartmentNumber = _PreviousNum;
            Capacity = _PrevCapacity;
            MonthlyCost = _PrevCost;
            Bedrooms = _PrevBedrooms;
            HasKitchen = _PrevKitchen;
            Bathrooms = _PrevBathrooms;

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
