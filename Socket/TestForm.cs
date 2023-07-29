using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace Socket
{
    public interface IFormBase
    {
        XMLManager XMLManager { get; set; }

        // 管理员对象
        Admin Admin { get; set; }
        void InitForm();
        // 响应btn事件
        void BtnClick(object sender, EventArgs e);
        // 创建form需要的组件
        void CreateComponent();
    }

    /// <summary>
    /// 界面基类,派生类仅仅需要重新定义initform和namestring这些数组还有btnclick事件即可
    /// </summary>
    public class FormBase : Form, IFormBase
    {
        // 设置管理员类为属性，这里会不会有强耦合
        // 初始密码和ID,每次初始化都把Admin属性初始化了
        private static Admin admin = new Admin("admin", "88888888");
        public Admin Admin
        {
            get
            {
                return admin;
            }
            set
            {
                admin = value;
            }
        }
        public XMLManager XMLManager { get; set; }

        #region Form属性,控件名称数组
        private string[] btnNameString = { };
        private string[] labelNameString = { };
        private string[] boxNameString = { };

        public string[] BtnNameString
        {
            get
            {
                return btnNameString;
            }
            set
            {
                btnNameString = value;
            }
        }
        public string[] LabelNameString
        {
            get
            {
                return labelNameString;
            }
            set
            {
                labelNameString = value;
            }
        }
        public string[] BoxNameString
        {
            get
            {
                return boxNameString;
            }

            set
            {
                boxNameString = value;
            }
        }
        #endregion

        #region form属性,控件列表
        private List<Label> labels = new List<Label>();
        public List<Label> Labels
        {
            set { labels = value; }
            get { return labels; }
        }

        private List<Button> buttons = new List<Button>();
        public List<Button> Buttons
        {
            set { buttons = value; }
            get { return buttons; }
        }

        private List<TextBox> textBoxes = new List<TextBox>();
        public List<TextBox> TextBoxes
        {
            set { textBoxes = value; }
            get { return textBoxes; }
        }
        #endregion

        public FormBase()
        {
            // 在这里加init就不行了，button不能获取文本，这是为什么
            // 是因为创建子类实例时，会先创建父类实例
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "FormBase";
            MessageBox.Show("FormBase构造函数被调用了");
        }
        public virtual void InitForm()
        {
            throw new NotImplementedException();
        }
        public virtual void CreateComponent()
        {

            int horizontalDistance = 5;
            int verticalDistance = 10;
            int i = 0;
            #region 生产label + box 的排列
            try
            {
                if (LabelNameString.Length != BoxNameString.Length)
                    throw new ArgumentException("label num doesn't match box num");
                // 循环初始化,label向下排开，box在label右边，也是向下排开
                for (i = 0; i < LabelNameString.Length; i++)
                {
                    Labels.Add(new Label()
                    {
                        Name = LabelNameString[i],
                        Text = LabelNameString[i].Replace("Label", ""),
                        Location = i == 0 ? new Point(10, 10) : new Point(Labels[i - 1].Left, Labels[i - 1].Bottom + verticalDistance),
                    });
                    this.Controls.Add(Labels[i]);
                }
                for (i = 0; i < BoxNameString.Length; i++)
                {
                    TextBoxes.Add(new TextBox()
                    {
                        Name = BoxNameString[i],
                        Location = new Point(Labels[i].Right + horizontalDistance, Labels[i].Top)
                    });
                    // 设置密码不显示
                    if (TextBoxes[i].Name.ToLower().Contains("password"))
                    {
                        TextBoxes[i].UseSystemPasswordChar = true;
                    }
                    this.Controls.Add(TextBoxes[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            #endregion
        }

        // 响应btn事件
        public virtual void BtnClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            #region 共有button事件
            if (button.Name.ToLower() == "QuitBtn".ToLower())
            {
                DialogResult result = MessageBox.Show("确认退出？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show(this.ToString() + "触发了Qquit Button");
                    this.Close();
                }
                else
                {
                }
            }
            else if (button.Name.ToLower() == "logOutBtn".ToLower())
            {
                this.Hide();
                using (var logInForm = new LogInForm())
                {
                    logInForm.ShowDialog();
                }

                this.Close();
            }
            else if (button.Name.ToLower() == "ReturnMainBtn".ToLower())
            {
                this.Hide();
                //mainForm.ShowDialog();
                using (var mainForm = new MainForm())
                {
                    mainForm.ShowDialog();
                }

                this.Close();
            }
            else if (button.Name.ToLower() == "LogInBtn".ToLower())
            {
                try
                {
                    // 必需第一个textbox为id,第二个为password
                    if (TextBoxes[0].Name.ToLower() != "IDBox".ToLower() || TextBoxes[1].Name.ToLower() != "PasswordBox".ToLower())
                        throw new Exception("ID和PasswordBOX顺序错误");
                    if (TextBoxes[0].Text == Admin.ID && TextBoxes[1].Text == Admin.Password)
                    {
                        MessageBox.Show("登录成功");
                        this.Hide();
                        using (var mainForm = new MainForm())
                        {
                            mainForm.ShowDialog();
                        }
                 
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("用户名或者密码错误");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (button.Name.ToLower() == "ViewBtn".ToLower())
            {
                this.Hide();
                // 打开查看界面
                using (var viewForm = new ViewForm())
                {
                    viewForm.ShowDialog();
                }
                MessageBox.Show(this.ToString()+"关闭了");
                this.Close();
            }
            else if (button.Name.ToLower() == "ReviseBtn".ToLower())
            {
                this.Hide();
                // 打开修改界面
                using (var reviseForm = new ReviseForm())
                {
                    reviseForm.ShowDialog();
                }
                 
                this.Close();
            }
            else if (button.Name.ToLower() == "AdminManagerBtn".ToLower())
            {
                this.Hide();
                // 打开修改界面
                using (var adminForm = new AdminForm())
                {
                    adminForm.ShowDialog();
                }
                 
                this.Close();
            }
            else if (button.Name.ToLower() == "PasswordReviseBtn".ToLower())
            {
                try
                {
                    if (TextBoxes[0].Name.ToLower() != "OriginalPasswordBox".ToLower() || textBoxes[1].Name.ToLower() != "NewPasswordBox".ToLower() || textBoxes[2].Name.ToLower() != "ConfirmPasswordBox".ToLower())
                        throw new Exception("密码修改填写顺序错误");
                    if (TextBoxes[0].Text.Equals(Admin.Password))
                    {
                        if (TextBoxes[1].Text == TextBoxes[2].Text)
                        {
                            Admin.Password = textBoxes[1].Text;
                            MessageBox.Show("密码修改成功");
                        }
                        else
                        {
                            MessageBox.Show("两次密码不同");
                        }
                    }
                    else
                    {
                        MessageBox.Show("原始密码错误");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            #endregion
        }
    }
    /// <summary>
    /// 登录基类
    /// </summary>
    public partial class LogInForm : FormBase, IFormBase
    {
        public LogInForm() : base()
        {
            InitializeComponent();
            this.Text = "LogInForm";
            InitForm();
        }

        public override void InitForm()
        {
            #region 组件名称
            string[] arr1 = { "LogInBtn", "QuitBtn" };
            BtnNameString = arr1;
            string[] arr2 = { "IDLabel", "PasswordLabel" };
            LabelNameString = arr2;
            string[] arr3 = { "IDBox", "PasswordBox" };
            BoxNameString = arr3;
            #endregion
            CreateComponent();
        }

        public override void CreateComponent()
        {
            base.CreateComponent();
            #region 独有控件,按钮
            for (int i = 0; i < BtnNameString.Length; i++)
            {
                Buttons.Add(new Button()
                {
                    Name = BtnNameString[i],
                    Text = BtnNameString[i].Replace("Btn", ""),
                    Location = i == 0 ? new Point(Labels[Labels.Count - 1].Left, Labels[Labels.Count - 1].Bottom + 20) : new Point(Buttons[i - 1].Right + 30, Buttons[i - 1].Top)
                });
                this.Controls.Add(Buttons[i]);
                Buttons[i].Click += BtnClick;
            }
            #endregion
        }
    }

    /// <summary>
    /// window 工厂类,工厂模式
    /// </summary>
    public class WindowFactory
    {
        private readonly object lockObj;
        private static WindowFactory instance;
        private WindowFactory()
        {
            instance = new WindowFactory();
        }

        public WindowFactory GetInstance()
        {
            if (instance == null)
            {
                // 这里为什么要加锁，加锁的对象是lockobj，是实例中的一个只读字段
                // 相当于对实例加锁
                lock (lockObj)
                {
                    if (instance == null)
                        instance = new WindowFactory();
                }
            }
            return instance;
        }

        public void ShowMess()
        {
            Console.WriteLine("获取到了单例工厂");
        }
    }
    public class MainForm : FormBase, IFormBase
    {
        public MainForm() : base()
        {
            this.Text = "MainForm";
            InitForm();
        }
        public override void InitForm()
        {
            string[] btnNameString = { "LogOutBtn", "ViewBtn", "ReviseBtn", "AdminManagerBtn", "QuitBtn" };
            BtnNameString = btnNameString;
            CreateComponent();
        }

        public override void CreateComponent()
        {
            base.CreateComponent();
            #region 独有控件,按钮
            for (int i = 0; i < BtnNameString.Length; i++)
            {
                Buttons.Add(new Button()
                {
                    Name = BtnNameString[i],
                    Text = BtnNameString[i].Replace("Btn", ""),
                    Location = i == 0 ? new Point(30, 30) : new Point(Buttons[i - 1].Right + 30, Buttons[i - 1].Top)
                });
                this.Controls.Add(Buttons[i]);
                Buttons[i].Click += BtnClick;
            }
            #endregion
        }
    }

    public class ViewForm : FormBase, IFormBase
    {
        private RichTextBox viewTextBox = new RichTextBox()
        {
            Width = 200,
            Height = 200,
            Text = ""
        };

        public ViewForm() : base()
        {
            this.Text = "ViewForm";

            InitForm();
        }

        public override void InitForm()
        {
            #region 组件名称
            string[] arr1 = { "ViewBtn", "ViewBytagBtn", "QuitBtn", "ReturnMainBtn", "LogOutBtn" };
            BtnNameString = arr1;
            string[] arr2 = { "FilepPathLabel", "TagLabel" };
            LabelNameString = arr2;
            string[] arr3 = { "PathBox", "TagBox" };
            BoxNameString = arr3;
            #endregion
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "ViewForm";
            CreateComponent();
        }

        public override void CreateComponent()
        {
            base.CreateComponent();
            #region 独有控件
            viewTextBox.Location = new Point(TextBoxes[LabelNameString.Length - 1].Right + 50, TextBoxes[LabelNameString.Length - 1].Top);
            this.Controls.Add(viewTextBox);

            for (int i = 0; i < BtnNameString.Length; i++)
            {
                Buttons.Add(new Button()
                {
                    Name = BtnNameString[i],
                    Text = BtnNameString[i].Replace("Btn", ""),
                    Location = i == 0 ? new Point(viewTextBox.Left + 30, viewTextBox.Bottom + 30) : new Point(Buttons[i - 1].Right + 30, Buttons[i - 1].Top)
                });
                this.Controls.Add(Buttons[i]);
                Buttons[i].Click += BtnClick;
            }
            #endregion
        }


        public override void BtnClick(object sender, EventArgs e)
        {
            base.BtnClick(sender, e);
            Button button = (Button)sender;
            #region XML阅读器
            try
            {
                if (button.Name.ToLower() == "ViewBtn".ToLower())
                {
                    XMLManager = new XMLManager(@"" + TextBoxes[0].Text);
                    viewTextBox.Text = XMLManager.GetAllTagText();
                }
                else if (button.Name.ToLower() == "ViewBytagBtn".ToLower())
                {
                    XMLManager = new XMLManager(@"" + TextBoxes[0].Text);
                    viewTextBox.Text = XMLManager.GetNodeTextByName(TextBoxes[1].Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            #endregion
        }
    }

    public class ReviseForm : FormBase, IFormBase
    {
        public ReviseForm() : base()
        {
            InitForm();
        }

    }

    public class AdminForm : FormBase, IFormBase
    {
        public AdminForm() : base()
        {
            this.Text = "AdminForm";

            InitForm();
        }

        public override void InitForm()
        {
            string[] labelName = { "OriginalPasswordLabel", "NewPasswordLabel", "ConfirmPasswordAgainLabel" };
            string[] boxname = { "OriginalPasswordBox", "NewPasswordBox", "ConfirmPasswordBox" };
            string[] btnName = { "PasswordReviseBtn", "ReturnMainBtn", "QuitBtn", "LogOutBtn" };
            LabelNameString = labelName;
            BoxNameString = boxname;
            BtnNameString = btnName;

            CreateComponent();
        }

        public override void CreateComponent()
        {
            base.CreateComponent();
            #region 独有控件,按钮
            for (int i = 0; i < BtnNameString.Length; i++)
            {
                Buttons.Add(new Button()
                {
                    Name = BtnNameString[i],
                    Text = BtnNameString[i].Replace("Btn", ""),
                    Location = i == 0 ? new Point(Labels[Labels.Count - 1].Left, Labels[Labels.Count - 1].Bottom + 20) : new Point(Buttons[i - 1].Right + 30, Buttons[i - 1].Top)
                });
                this.Controls.Add(Buttons[i]);
                Buttons[i].Click += BtnClick;
            }
            #endregion
        }
    }
}
