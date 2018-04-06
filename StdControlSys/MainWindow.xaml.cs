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
using System.Threading;
using System.ComponentModel;

namespace StdControlSys
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Global GlobalInfo = new Global();
        Random random = new Random();
        List<Std> stds = new List<Std>();
        List<Group> groups = new List<Group>();
        public string FlowersWords = "";
        public string FlowerWordsDefault = "春夏秋冬江花月夜雪山行云雨";
        public MainWindow()
        {
            InitializeComponent();
            ReadFileData("GroupInfo.xml", "StdInfo_GY4.xml");
            FlowersWords = FlowerWordsDefault;
            foreach(var g in groups)
            {
                MessageBox.Show(g.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectedGroup.DataContext = GlobalInfo;
            this.SelectedStd.DataContext = GlobalInfo;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="GroupFileName">组文件位置</param>
        /// <param name="StdFileName">学生文件位置</param>
        public void ReadFileData(string GroupFileName,string StdFileName)
        {
            if (!File.Exists(GroupFileName) || !File.Exists(StdFileName)) 
            {
                MessageBox.Show( !File.Exists(GroupFileName) ? GroupFileName : StdFileName + "文件不存在", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
            groupinfos = null;
        }

        public void ShowGroups()
        {
            
        }

        private void FlowerTokenButton_Click(object sender, RoutedEventArgs e)
        {
            if (FlowersWords.Length==0) { FlowersWords = FlowerWordsDefault; FlowerTokenWord.Content = "完"; return; }
            char Selected=FlowersWords.ElementAtOrDefault(random.Next(0,FlowersWords.Length - 1));
            FlowerTokenWord.Content = Selected;
            FlowersWords=FlowersWords.Remove(FlowersWords.IndexOf(Selected), 1);
        }

        private void RandomGroupButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadStart threadstart = new ThreadStart(RandomGroup);
            Thread thread = new Thread(threadstart);
            thread.Start();
        }

        /// <summary>
        /// 随机抽取小组
        /// </summary>
        private void RandomGroup()
        {
            Group group;
            for(int i = 0; i < 10; i++)
            {
                group = groups.ElementAtOrDefault(random.Next(0, groups.Count));
                GlobalInfo.SelectedGroupInfo = "第" + group.Order + "组\n" + group.Name + "\n组长:" + group.Leader.Name;
                Thread.Sleep(20 + i * i * 5);
            }
        }

        private void RandomStdButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadStart threadstart = new ThreadStart(RandomStd);
            Thread thread = new Thread(threadstart);
            thread.Start();
        }

        /// <summary>
        /// 随机抽取学生
        /// </summary>
        private void RandomStd()
        {
            Std std;
            for (int i = -15; i < 15; i++)
            {
                std = stds.ElementAtOrDefault(random.Next(0, stds.Count));
                GlobalInfo.SelectedStdInfo = std.Name + "\n" + std.Number;
                Thread.Sleep(20 + i * i);
            }
        }
    }
}
