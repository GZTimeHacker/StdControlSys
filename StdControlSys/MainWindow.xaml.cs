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
using System.IO;

namespace StdControlSys
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        List<Std> stds = new List<Std>();
        List<Group> groups = new List<Group>();
        public MainWindow()
        {
            InitializeComponent();
            ReadFileData("GroupInfo.xml", "StdInfo_GY4.xml");
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="GroupFileName">组文件位置</param>
        /// <param name="StdFileName">学生文件位置</param>
        public void ReadFileData(string GroupFileName,string StdFileName)
        {
            stds = null;
            groups = null;
            groups = new List<Group>();
            #region 写入测试
            //List<GroupInfo> ingroups = new List<GroupInfo>();
            //ingroups.Add(new GroupInfo("第一组", 1, 170401, new int[] { 170402, 170403, 170404, 170405, 170406, 170407 }));
            //ingroups.Add(new GroupInfo("SiO2", 2, 170411, new int[] { 170412, 170413, 170414, 170415, 170416, 170417 }));
            //ingroups.Add(new GroupInfo("如来佛祖", 3, 170421, new int[] { 170422, 170423, 170424, 170425, 170426, 170427 }));
            //if (Info.SerializerGrp(ingroups, "GroupInfo.xml")) MessageBox.Show("Done");
            #endregion
            List<GroupInfo> groupinfos;
            groupinfos = Info.DeserializeGrp_List(GroupFileName);
            stds = Info.DeserializeStd_List(StdFileName);
            foreach (var item in groupinfos)
            {
                groups.Add(new Group(stds, item));
            }
            /* 
             * foreach (var item in groups)
             * {
             *     MessageBox.Show(item.ToString());
             * }
             */
            groupinfos = null;
        }

    }
}
