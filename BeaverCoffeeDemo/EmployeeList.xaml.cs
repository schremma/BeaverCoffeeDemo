using BeaverCoffeeDemo.Data;
using BeaverCoffeeDemo.ValidationRules;
using BeaverCoffeeDemo.ViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for EmployeeList.xaml
    /// </summary>
    public partial class EmployeeList : UserControl, ISwitchable
    {
        private ObservableCollection<EmployeeViewModel> _Employees;
        private ObservableCollection<Location> _Locations;
        private ObservableCollection<PositionViewModel> _EmployeesPositions;

        private Location currentLocation;

        private List<EmployeeViewModel> newEmployees;
        private List<PositionViewModel> newPositions;


        public EmployeeList()
        {
            InitializeComponent();
            currentLocation = ShopGlobals.Location;

            GetLocations();
            this.DataContext = this;

        }

        public ObservableCollection<EmployeeViewModel> Employees
        { get { return _Employees; } }

        public ObservableCollection<Location> Locations
        { get { return _Locations; } }

        public ObservableCollection<PositionViewModel> EmployeesPositions
        { get { return _EmployeesPositions; } }



        private void Employees_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                newEmployees = e.NewItems.Cast<EmployeeViewModel>().ToList();
            }

            if (e.OldItems != null)
            {
                foreach (EmployeeViewModel employee in e.OldItems)
                {
                    employee.PropertyChanged -= Employee_PropertyChanged;
                    employee.Address.PropertyChanged -= Address_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (EmployeeViewModel employee in e.NewItems)
                {
                    employee.PropertyChanged += Employee_PropertyChanged;
                    employee.Address.PropertyChanged += Address_PropertyChanged;
                }
            }
        }

        private void Address_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           
            AddressViewModel address = sender as AddressViewModel;
            if (address.OwnerId != null && address.OwnerId != ObjectId.Empty)
            {
                var propertyValue = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
                string propertyName = "Address." + e.PropertyName;

                if (!DbManager.GetInstance().UpdateEmployeeProperty(propertyValue, propertyName, address.OwnerId))
                {
                    MessageBox.Show("Failed to update property in database");
                }
            }
        }

        private void Employee_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            EmployeeViewModel employee = sender as EmployeeViewModel;
            if (employee.Id != null && employee.Id != ObjectId.Empty)                                
            {
                    var propertyValue = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
                    bool ok = true;
                    if (e.PropertyName.Equals("PersonalId"))
                    {
                        if (employee.AssociatedLocation != null && !string.IsNullOrEmpty(employee.AssociatedLocation.PersonalIdFormat))
                        {
                            ok = ValidationHelper.ValidateIdFormat(employee.AssociatedLocation.PersonalIdFormat, propertyValue.ToString());
                        }
                        else
                            ok = false;
                    }
                    if (ok && !DbManager.GetInstance().UpdateEmployeeProperty(propertyValue, e.PropertyName, employee.Id))
                    {
                        MessageBox.Show("Failed to update property in database or property did not change");
                    }
            }
        }

        private void AddNewEmployees(IList<EmployeeViewModel> employees)
        {
            foreach (var e in employees)
            {
                DbManager.GetInstance().AddEmployee(ViewModelToEmployee(e));
            }
        }

        private void GetLocations()
        {
            var locations = DbManager.GetInstance().GetAllLocations();
            _Locations = new ObservableCollection<Location>(locations);

        }

        private void GetEmployeesForLocation(Location location)
        {
            var employees = DbManager.GetInstance().GetEmployeesForLocation(location);
            LoadEmployees(employees);

        }

        private void GetAllEmployees()
        {
            var employees = DbManager.GetInstance().GetAllEmployees();
            LoadEmployees(employees);
        }

        private void LoadEmployees(IEnumerable<Employee> employees)
        {
            _Employees = new ObservableCollection<EmployeeViewModel>();
            foreach (var e in employees)
            {
                _Employees.Add(EmployeeToViewModel(e));
            }
            dgEmployees.ItemsSource = _Employees;

            _Employees.CollectionChanged += Employees_CollectionChanged;

            foreach (EmployeeViewModel employee in Employees)
            {
                employee.PropertyChanged += Employee_PropertyChanged;
                employee.Address.PropertyChanged += Address_PropertyChanged;
            }
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void cbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Location loc = cbLocation.SelectedItem as Location;
            currentLocation = loc;
            if (loc.Id != ObjectId.Empty)
            {
                GetEmployeesForLocation(loc);
                ShowShopsForLocation(loc);

            }
            else
            {
                GetAllEmployees();
            }
            if (EmployeesPositions != null)
            {
                EmployeesPositions.Clear();
            }

        }

        private void ShowShopsForLocation(Location loc)
        {
            List<ShopViewModel> shopList = new List<ShopViewModel>();

            if (loc.Id != ObjectId.Empty)
            {
                currentLocation = loc;
                shopList.Add(new ShopViewModel()
                {
                    Id = ObjectId.Empty,
                    ShopDescription = "No shop"
                });

                foreach (var s in loc.Shops)
                {
                    shopList.Add(ShopToViewModel(s));
                }

            }

            cbShops.ItemsSource = shopList;
        }

        private void dgEmployees_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new EmployeeViewModel()
            {
                HireDate = DateTime.Now,
                Address = new AddressViewModel()
                {
                    Country = currentLocation.Name
                },
                AssociatedLocation = currentLocation,
            };
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem != null)
            {
                EmployeeViewModel toDelete = dgEmployees.SelectedItem as EmployeeViewModel;
                IList<EmployementPosition> empPositions = Employees.Where(emp => emp.Id == toDelete.Id).Single().Positions.ToList();

                Employees.Remove(toDelete);
                DbManager.GetInstance().RemoveEmployee(toDelete.Id);
                dgEmployees.ItemsSource = Employees;

                foreach (var pos in empPositions)
                {
                    var theRemovedPosition = EmployeesPositions.Where(p => p.Equals(pos)).FirstOrDefault();
                    if (theRemovedPosition != null)
                        EmployeesPositions.Remove(theRemovedPosition);
                }
                dgPositions.ItemsSource = null;
                dgPositions.ItemsSource = EmployeesPositions;
            }
        }

        private bool EmployeeListHasError()
        {
            var itemsSource = dgEmployees.ItemsSource as IEnumerable<EmployeeViewModel>;
            bool hasError = true;
            foreach (var item in itemsSource)
            {
                var row = dgEmployees.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (row != null)
                {
                    hasError = !IsValid(row);
                    if (hasError)
                        return hasError;
                }
            }
            return hasError;
        }

        private bool PositionListHasError()
        {
            var itemsSource = dgPositions.ItemsSource as IEnumerable<PositionViewModel>;
            bool hasError = true;
            foreach (var item in itemsSource)
            {
                var row = dgPositions.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (row != null)
                {
                    hasError = !IsValid(row);
                    if (hasError)
                        return hasError;
                }
            }
            return hasError;
        }

        private bool IsValid(DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return false;

            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child)) { return false; }
            }

            return true;
        }

        private void dgEmployees_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (!EmployeeListHasError())
                {
                    var newItem = e.Row.Item;

                    if (newEmployees != null && newEmployees.Contains(newItem))
                    {
                        bool ok = true;
                        ObjectId id = ObjectId.Empty;
                        foreach (var emp in newEmployees)
                        {
                            id = DbManager.GetInstance().AddEmployee(ViewModelToEmployee(emp));
                            if (id == null || id == ObjectId.Empty)
                            {
                                ok = false;
                            }
                            else
                            {
                                emp.Id = id;
                            }
                        }
                        if (!ok)
                        {
                            MessageBox.Show("Adding new employee(s) failed");
                        }
                        else
                        {
                            newEmployees.Clear();
                        }
                    }
                }

            }
        }


        private void dgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowCoffeeShopLocationEditor(false);

            EmployeeViewModel selected = dgEmployees.SelectedItem as EmployeeViewModel;
            if (selected != null)
            {
                _EmployeesPositions = new ObservableCollection<PositionViewModel>();
                _EmployeesPositions.CollectionChanged += _EmployeesPositions_CollectionChanged;

                var editableCollection = (IEditableCollectionView)dgPositions.Items;

                var positions = DbManager.GetInstance().GetPositionsOfEmployee(selected.Id);
                if (positions != null)
                {
                    foreach (var pos in positions)
                    {
                        PositionViewModel vm = PositionToViewModel(pos);
                        vm.EmployeeId = selected.Id;
                        _EmployeesPositions.Add(vm);
                    }
                }
                else
                {
                    _EmployeesPositions = new ObservableCollection<PositionViewModel>();
                }
                dgPositions.ItemsSource = _EmployeesPositions;

            }
        }

        private void ShowCoffeeShopLocationEditor(bool show)
        {
            containerCoffeeShop.Visibility = show ? Visibility.Visible : Visibility.Hidden;
        }

        private void _EmployeesPositions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var positionList = e.NewItems.Cast<PositionViewModel>().ToList();
                foreach (var position in positionList)
                {
                    if (position.Id == null || position.Id.Equals(ObjectId.Empty))
                    {
                        if (newPositions == null)
                            newPositions = new List<PositionViewModel>();
                        newPositions.Add(position);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (PositionViewModel position in e.OldItems)
                {
                    position.PropertyChanged -= Position_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (PositionViewModel position in e.NewItems)
                {
                    position.PropertyChanged += Position_PropertyChanged;
                }
            }
        }

        private void Position_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PositionViewModel position = sender as PositionViewModel;
            if (position.Id != null && position.Id != ObjectId.Empty)
            {
                var propertyValue = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
                string propertyName = e.PropertyName;

                if (ShouldUpdatePositionProperty(propertyName))
                {
                    if (!DbManager.GetInstance().UpdatePPositionProperty(propertyValue, propertyName, position.Id))
                    {
                        MessageBox.Show("Failed to update property in database");
                    }
                }
            }
        }

        private void dgPositions_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {

            Location loc = cbLocation.SelectedItem as Location;
            EmployeeViewModel emp = dgEmployees.SelectedItem as EmployeeViewModel;

            if (emp != null)
            {
                e.NewItem = new PositionViewModel()
                {
                    StartDate = DateTime.Now,
                    Position = 1,
                    FullTimeProcent = 100,
                    WorkLocation = loc.Id != ObjectId.Empty ? loc.Name : "",
                    EmployeeId = emp != null ? emp.Id : ObjectId.Empty
                };
            }
            else
            {
                MessageBox.Show("Selected an employee first to add the position to");
            }

        }

        private void dgPositions_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (!PositionListHasError())
                {
                    var newItem = e.Row.Item;
               
                    if (newPositions != null && newPositions.Contains(newItem))
                    {
                        bool ok = true;
                        foreach (var pos in newPositions)
                        {
                            if (pos.EmployeeId == null || pos.EmployeeId == ObjectId.Empty)
                                ok = false;
                            else
                            {
                                EmployementPosition empPosition = ViewModelToPosition(pos);
                                ok = DbManager.GetInstance().AddPositionToEmployee(empPosition, pos.EmployeeId);
                                if (ok)
                                {
                                    var positions = Employees.Where(emp => emp.Id == pos.EmployeeId).Single().Positions.ToList();
                                    positions.Add(empPosition);
                                    Employees.Where(emp => emp.Id == pos.EmployeeId).Single().Positions = positions;
                                }
                            }
                        }
                        if (!ok)
                        {
                            MessageBox.Show("Adding new position failed");
                        }
                        else
                        {
     
                            newPositions.Clear();
                        }
                    }
                }

            }
        }

        private void btnDeletePosition_Click(object sender, RoutedEventArgs e)
        {
            PositionViewModel pos = dgPositions.SelectedItem as PositionViewModel;
            if (pos != null)
            {
                ShowCoffeeShopLocationEditor(false);
                if (DbManager.GetInstance().RemovePositionFromEmployee(ViewModelToPosition(pos), pos.EmployeeId))
                {
                    IList<EmployementPosition> empPosition = Employees.Where(emp => emp.Id == pos.EmployeeId).Single().Positions.ToList();
                    var theRemovedPosition = empPosition.Where(p => pos.Equals(p)).FirstOrDefault();
                    if (theRemovedPosition != null)
                    {
                        empPosition.Remove(theRemovedPosition);
                        Employees.Where(emp => emp.Id == pos.EmployeeId).Single().Positions = empPosition;
                    }
                    EmployeesPositions.Remove(pos);
                    dgPositions.ItemsSource = EmployeesPositions;
                }
                else
                {
                    MessageBox.Show("Deleting position was unsuccessful");
                }
            }
        }

        private void dgPositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PositionViewModel position = dgPositions.SelectedItem as PositionViewModel;
            if (position != null)
            {
                ShowCoffeeShopLocationEditor(true);
                DisplayCoffeeShopInfo(position.ShopAddress);
            }
        }

        private void btnAddShopToPosition_Click(object sender, RoutedEventArgs e)
        {
            ShopViewModel shop = cbShops.SelectedItem as ShopViewModel;
            if (shop != null)
            {
                PositionViewModel pos = dgPositions.SelectedItem as PositionViewModel;
                if (pos != null)
                {
                    pos.ShopId = shop.Id;
                    pos.ShopAddress = shop.ShopAddress;
                    DisplayCoffeeShopInfo(pos.ShopAddress);

                    MessageBox.Show("Shop has been associated with position");
                }
            }

        }

        private void DisplayCoffeeShopInfo(AddressViewModel shopAddress)
        {
            if (shopAddress != null)
            {
                lblShopAssociatedWithPosition.Content = "Current coffee shop = " + shopAddress.ToString();
            }
            else
            {
                lblShopAssociatedWithPosition.Content = "This position is not associated with a specific coffee shop";
            }
        }

        private EmployeeViewModel EmployeeToViewModel(Employee emp)
        {
            EmployeeViewModel vm = new EmployeeViewModel()
            {
                Address = new AddressViewModel()
                {
                    OwnerId = emp.Id,
                    City = emp.Address.City,
                    Country = emp.Address.Country,
                    Street = emp.Address.Street,
                    Zip = emp.Address.Zip
                },
                EmployementEndDate = emp.EmployementEndDate,
                HireDate = emp.HireDate,
                Id = emp.Id,
                Name = emp.Name,
                PersonalId = emp.PersonalId,
                Positions = emp.Positions,
                AssociatedLocation = currentLocation.Id != ObjectId.Empty ? currentLocation : null,

            };
            return vm;
        }

        private Employee ViewModelToEmployee(EmployeeViewModel vm)
        {
            Employee employee = new Employee()
            {
                Address = new Address()
                {
                    City = vm.Address.City,
                    Country = vm.Address.Country,
                    Street = vm.Address.Street,
                    Zip = vm.Address.Zip
                },
                EmployementEndDate = vm.EmployementEndDate,
                HireDate = vm.HireDate,
                Id = vm.Id,
                Name = vm.Name,
                PersonalId = vm.PersonalId,
                Positions = vm.Positions,
                Comments = new List<Comment>(),
            };
            return employee;
        }

        private PositionViewModel PositionToViewModel(EmployementPosition pos)
        {
            PositionViewModel vm = new PositionViewModel()
            {
                BossOver = pos.BossOver,
                EndDate = pos.EndDate,
                FullTimeProcent = pos.FullTimeProcent,
                Id = pos.Id,
                ShopId = pos.ShopId,
                StartDate = pos.StartDate,
                Position = pos.Position,
                WorkLocation = DbManager.GetInstance().GetNameOfLocation(pos.WorkLocationId),
            };

            Address address = DbManager.GetInstance().GetShopAddressForId(pos.ShopId);
            if (address !=null)
            {
                vm.ShopAddress = AddressToViewModel(address);
            }

            return vm;
        }

        private AddressViewModel AddressToViewModel(Address addr)
        {
            AddressViewModel vm = new AddressViewModel()
            {
                City = addr.City,
                Country = addr.Country,
                Street = addr.Street,
                Zip = addr.Zip
            };

            return vm;
        }

        private EmployementPosition ViewModelToPosition(PositionViewModel vm)
        {
            EmployementPosition p = new EmployementPosition();

            Location workLoc = DbManager.GetInstance().GetLocationForName(vm.WorkLocation);

            p.BossOver = vm.BossOver != null ? vm.BossOver : null;
            p.EndDate = vm.EndDate;
            p.FullTimeProcent = vm.FullTimeProcent;
            p.Id = vm.Id;
            p.ShopId = vm.ShopId;
            p.StartDate = vm.StartDate;
            p.Position = vm.Position;
            p.WorkLocationId = workLoc != null ? workLoc.Id : ObjectId.Empty;


            return p;
        }

        private ShopViewModel ShopToViewModel(Shop shop)
        {
            ShopViewModel vm = new ShopViewModel()
            {
                Id = shop.Id,
                ShopDescription = shop.Address.ToString()
            };


            Address address = DbManager.GetInstance().GetShopAddressForId(shop.Id);
            if (address != null)
            {
                vm.ShopAddress = AddressToViewModel(address);
            }
            return vm;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private bool ShouldUpdatePositionProperty(string propertyName)
        {
            EmployementPosition p = new EmployementPosition();
            Type type = p.GetType();
            return type.GetProperty(propertyName) != null;
        }

    }
}
