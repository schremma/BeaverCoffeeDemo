using BeaverCoffeeDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for AddComment.xaml
    /// </summary>
    public partial class AddComment : UserControl
    {

        private List<Employee> _Employees = new List<Employee>();
        private List<Comment> _Comments = new List<Comment>();
        private Comment _Comment = new Comment();

        public AddComment()
        {
            InitializeComponent();
            if (ShopGlobals.Employee.Positions.Any(p => p.BossOver != null))
                GetEmployeesOfBoss(ShopGlobals.Employee);
            EnableCommenting(false);
            this.DataContext = _Comment;
        }

        public List<Employee> Employees
        { get { return _Employees; } }

        public List<Comment> Comments
        { get { return _Comments; } }
        

        private void GetEmployeesOfBoss(Employee boss)
        {
            if (Employees.Count > 0)
            {
                _Employees.Clear();
                cbEmployees.ItemsSource = null;
            }

            var employees = boss.Positions.Where(p => p.BossOver != null && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)).SelectMany(p => p.BossOver);

            DbManager db = DbManager.GetInstance();
            foreach (var e in employees)
            {
                _Employees.Add(db.GetEmployee(e));
            }
            cbEmployees.ItemsSource = Employees;
        }
        private void EnableCommenting(bool enable)
        {
            tbCommentText.IsEnabled = enable;
            btnAddComment.IsEnabled = enable;
        }

        private void LoadComments(Employee employee)
        {
            Comments.Clear();
            dgComments.ItemsSource = null;
            if (employee.Comments != null)
            {
                foreach (var c in employee.Comments)
                {
                    Comments.Add(c);
                }
            }
            dgComments.ItemsSource = Comments;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void btnAddComment_Click(object sender, RoutedEventArgs e)
        {
            if (!System.Windows.Controls.Validation.GetHasError(tbCommentText))
            {
                SaveComment();
            }
        }

        private void SaveComment()
        {
            if (_Comment != null)
            {
                Employee selectedEmployee = cbEmployees.SelectedItem as Employee;
                if (selectedEmployee != null)
                {
                    _Comment.Date = DateTime.UtcNow;
                    _Comment.EmployerId = ShopGlobals.Employee.Id;
                    _Comment.EmployerName = ShopGlobals.Employee.Name;

                    if (DbManager.GetInstance().AddCommentToEmployee(_Comment, selectedEmployee.Id))
                    {

                        if (selectedEmployee.Comments == null)
                        {
                            selectedEmployee.Comments = new List<Comment>();
                        }

                        Comment newComment = new Comment()
                        {
                            Date = _Comment.Date,
                            EmployerId = _Comment.EmployerId,
                            EmployerName = _Comment.EmployerName,
                            Text = _Comment.Text,
                        };
                        selectedEmployee.Comments.Add(newComment);
                        _Comments.Add(newComment);
                        _Comment = new Comment();
                        this.DataContext = _Comment;
                        dgComments.ItemsSource = null;
                        dgComments.ItemsSource = Comments;

                        MessageBox.Show("Comment has been saved");
                    }
                    else
                        MessageBox.Show("Error saving comment in database: " + DbManager.GetInstance().Error);
                }
            }

        }


        private void cbEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee selected = cbEmployees.SelectedItem as Employee;
            if (selected != null)
            {
                LoadComments(selected);
                
                EnableCommenting(true);
            }
        }
    }
}
