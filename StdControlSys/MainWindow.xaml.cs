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
using System.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;

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
        public string FlowerWordsDefault = "春夏秋冬江花月夜雪山行云雨水";
        int SelectingStdNum = -1;
        int SelectingGrpOrder = -1;
        int SelectingStdInGrp = -1;
        string StdLocal = @"Data\StdInfo.xml";
        string GrpLocal = @"Data\GroupInfo.xml";
        string ScLocal = @"Data\ScoreInfo.xml";
        bool isFile = true;

        #region 后台启动工作
        public MainWindow()
        {
            InitializeComponent();
            if(!ReadFileData(GrpLocal, StdLocal)) Close();
            FlowersWords = FlowerWordsDefault;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectedGroup.DataContext = GlobalInfo;
            this.SelectedStd.DataContext = GlobalInfo;
            this.FlowerTokenWord.DataContext = GlobalInfo;
            this.GroupData.ItemsSource = groups;
            this.SelectedStdInGrp.DataContext = GlobalInfo;
            this.AutoStdInGrp.DataContext = GlobalInfo;
            this.AddStdWithInfo.DataContext = GlobalInfo;
            this.AddScoreResultBox.DataContext = GlobalInfo;
            this.AddScoreInfoBox.DataContext = GlobalInfo;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="GroupFileName">组文件位置</param>
        /// <param name="StdFileName">学生文件位置</param>
        public bool ReadFileData(string GroupFileName,string StdFileName)
        {
            stds = new List<Std>();
            groups = new List<Group>();
            if (!File.Exists(GroupFileName) || !File.Exists(StdFileName)) 
            {
                MessageBox.Show( (!File.Exists(GroupFileName) ? GroupFileName : StdFileName )+ "文件不存在", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                isFile = false;
                return false;
            }
            #region 写入测试
            //List<GroupInfo> ingroups = new List<GroupInfo>();
            //ingroups.Add(new GroupInfo("第一组", 1, 170401, new int[] { 170402, 170403, 170404, 170405, 170406, 170407 }));
            //ingroups.Add(new GroupInfo("SiO2", 2, 170411, new int[] { 170412, 170413, 170414, 170415, 170416, 170417 }));
            //ingroups.Add(new GroupInfo("如来佛祖", 3, 170421, new int[] { 170422, 170423, 170424, 170425, 170426, 170427 }));
            //if (Info.SerializerGrp(ingroups, GrpLocal)) MessageBox.Show("Done");
            #endregion
            List<GroupInfo> groupinfos;
            groupinfos = Info.DeserializeGrp_List(GroupFileName);
            stds = Info.DeserializeStd_List(StdFileName);
            foreach (var item in groupinfos)
            {
                groups.Add(new Group(stds, item));
            }
            groupinfos = null;
            return true;
        }
        #endregion

        #region 飞花令
        private void FlowerTokenButton_Click(object sender, RoutedEventArgs e)
        {
            lock (GlobalInfo.FlowerTokenChar)
            {
            ThreadStart threadstart = new ThreadStart(FlowersToken);
            Thread thread = new Thread(threadstart);
            thread.Start();
            }
        }
        /// <summary>
        /// 飞花令随机汉字
        /// </summary>
        private void FlowersToken()
        {
            if (FlowersWords.Length == 0) { FlowersWords = FlowerWordsDefault; GlobalInfo.FlowerTokenChar = "完" ; return; }
            for (int i = -9; i < 10; i++)
            {
                GlobalInfo.FlowerTokenChar = Tools.CreateCode(1);
                Thread.Sleep(20 + i * i / 2 );
            }
            char Selected = FlowersWords.ElementAtOrDefault(random.Next(0, FlowersWords.Length - 1));
            GlobalInfo.FlowerTokenChar = Selected.ToString();
            FlowersWords = FlowersWords.Remove(FlowersWords.IndexOf(Selected), 1);
        }

        #endregion

        #region 随机抽取
        private void RandomGroupButton_Click(object sender, RoutedEventArgs e)
        {
            lock (GlobalInfo.SelectedGroupInfo)
            {
            ThreadStart threadstart = new ThreadStart(RandomGroup);
            Thread thread = new Thread(threadstart);
            thread.Start();
            }
        }
        /// <summary>
        /// 随机抽取小组
        /// </summary>
        private void RandomGroup()
        {
            Group group;
            for(int i = -15; i < 15; i++)
            {
                group = groups.ElementAtOrDefault(random.Next(0, groups.Count));
                GlobalInfo.SelectedGroupInfo = "第" + group.Order + "组\n" + group.Name + "\n组长:" + group.Leader.Name;
                Thread.Sleep(20 + i * i / 2);
                SelectingGrpOrder = group.Order;
            }
            if (GlobalInfo.IsAutoStdFromGroup)
            {
                lock (GlobalInfo.SelectedStdInGrpInfo)
                {
                    ThreadStart threadstart = new ThreadStart(RandomStdInGrp);
                    Thread thread = new Thread(threadstart);
                    thread.Start();
                }
            }
        }

        private void RandomStdButton_Click(object sender, RoutedEventArgs e)
        {
            lock (GlobalInfo.SelectedStdInfo)
            {
            ThreadStart threadstart = new ThreadStart(RandomStd);
            Thread thread = new Thread(threadstart);
            thread.Start();
            }
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
                Group grp = groups.Find(g => g.Order == std.Group);
                GlobalInfo.SelectedStdInfo = std.Name + "\n" + std.Number + "\n" + ((grp != null) ? grp.Name : "无组别");
                Thread.Sleep(20 + i * i / 2);
                SelectingStdNum = std.Number;
                grp = null;
            }
            std = null;
        }

        private void RandomStdInGrpButton_Click(object sender, RoutedEventArgs e)
        {
            lock (GlobalInfo.SelectedStdInGrpInfo)
            {
                ThreadStart threadstart = new ThreadStart(RandomStdInGrp);
                Thread thread = new Thread(threadstart);
                thread.Start();
            }
        }
        /// <summary>
        /// 随机抽取组内成员
        /// </summary>
        private void RandomStdInGrp()
        {
            if (SelectingGrpOrder == -1) { MessageBox.Show("请先抽取小组", "提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            Std std;
            Group grp = groups.Find(g => g.Order == SelectingGrpOrder);
            List<Std> stdl = grp.Members;
            stdl.Add(grp.Leader);
            stdl.Reverse();
            for (int i = -5; i < 15; i++)
            {
                std = stdl.ElementAt(random.Next(0, stdl.Count));
                GlobalInfo.SelectedStdInGrpInfo = "抽取中...\n"+std.Name + "\n" + std.Number;
                Thread.Sleep(20 + i * i /2);
                SelectingStdInGrp = std.Number;
            }
            std = stdl.ElementAt(random.Next(0,stdl.Count));
            GlobalInfo.SelectedStdInGrpInfo = "就是你了！\n"+std.Name + "\n" + std.Number;
            std = null;
            stdl = null;
            grp = null;
        }
        #endregion

        private void AddStdScore_Click(object sender, RoutedEventArgs e)
        {
            AddScoreToGrpStd(SelectingStdNum);
        }

        private void AddStdInGrpScore_Click(object sender, RoutedEventArgs e)
        {
            AddScoreToGrpStd(SelectingStdInGrp);
        }

        private void AddScoreToGrpStd(int StdNum)
        {
            if (StdNum == -1) { return; }
            stds.Find(s => s.Number == StdNum).Score++;
            Group grp = groups.Find(g => g.Leader.Number == StdNum);
            if (grp != null) { grp.Score++; return; }
            foreach (var gr in groups)
            {
                Std stemp = gr.Members.Find(s => s.Number == StdNum);
                if (stemp != null) { grp = groups.Find(g => g.Order == stemp.Group); break; }
            }
            if (grp != null) grp.Score++;
        }

        private void SaveScoreDataToFile()
        {
            List<int> GrpScore = new List<int>();
            List<int> StdScore = new List<int>();
            foreach(var g in groups)
            {
                GrpScore.Add(g.Score);
            }
            foreach(var s in stds)
            {
                StdScore.Add(s.Score);
            }
            ScoreData data = new ScoreData(Tools.GetMD5HashFromFile(GrpLocal),
                                                                    Tools.GetMD5HashFromFile(StdLocal),
                                                                    GrpScore, StdScore);
            if (Info.SerializerScore(data, ScLocal)) MessageBox.Show("保存完成！");
            else MessageBox.Show("保存失败。");
            data = null;
            GrpScore = null;
            StdScore = null;
        }

        /*
         * 1.
         * 2.添加得分前五同学公示
         * 3.DataGrid打乱顺序排列
         * 4.倒计时做成多线程分离窗口式组件
         * 5.
         */
        protected override void OnClosing(CancelEventArgs e)
        {
            if (isFile)
            {
                switch (MessageBox.Show("是否保存积分结果?", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
                {
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.Yes:
                        SaveScoreDataToFile();
                        break;
                    default:
                        break;
                }
            }
        }

        private void SaveScoreData_Click(object sender, RoutedEventArgs e)
        {
            SaveScoreDataToFile();
        }

        private void ReadScoreData_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(ScLocal)) { MessageBox.Show("文件不存在"); return; }
            if (MessageBox.Show(" 导入数据会覆盖现在的数据，是否继续？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel) return;
            ScoreData data = Info.DeserializeScore(ScLocal);
            if(data.GroupInfoMD5 != Tools.GetMD5HashFromFile(GrpLocal) || 
                data.StdDataMD5 != Tools.GetMD5HashFromFile(StdLocal))
            {
                if (MessageBox.Show("积分文件和小组或学生数据文件不匹配，是否继续导入?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information)
                    == MessageBoxResult.No) return;
            }
            foreach(var g in groups)
            {
                g.Score = data.GroupScore.FirstOrDefault();
                data.GroupScore.RemoveAt(0);
            }
            foreach (var s in stds)
            {
                s.Score = data.StudentScore.FirstOrDefault();
                data.StudentScore.RemoveAt(0);
            }
            data = null;
        }

        private void ResetScore_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(" 此操作会清空现在的积分，是否继续？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel) return;
            foreach (var item in groups)
            {
                item.Score = 0;
            }
            foreach (var item in stds)
            {
                item.Score = 0;
            }
            MessageBox.Show("清空完成");
        }

        private void ReadStdData_Click(object sender, RoutedEventArgs e)
        {
            if (ReadFileData(GrpLocal, StdLocal)) MessageBox.Show("读取成功");
            else MessageBox.Show("读取失败");
        }

        /// <summary>
        /// 为指定学生加分
        /// </summary>
        private void AddStdWithInfo_Click(object sender, RoutedEventArgs e)
        {
            int StdNum = 0;
            if(int.TryParse(GlobalInfo.AddScoreStdInfo,out StdNum))
            {
                Std stemp = stds.Find(s => s.Number == StdNum);
                if (stemp != null)
                {
                    stemp.Score++;
                    Group grp = groups.Find(g => g.Leader.Number == StdNum);
                    if (grp != null)
                    {
                        grp.Score++;
                        GlobalInfo.AddScoreResult = "已为\n" + stemp.Name + "及其小组积分";
                        return;
                    }
                    foreach (var gr in groups)
                    {
                        stemp = gr.Members.Find(s => s.Number == StdNum);
                        if (stemp != null) { grp = groups.Find(g => g.Order == stemp.Group); break; }
                    }
                    if (grp != null) grp.Score++;
                    GlobalInfo.AddScoreResult = "已为\n" + stemp.Name + "及其小组积分";
                    return;
                }
                else
                {
                    GlobalInfo.AddScoreResult = "未找到\n学号为" + StdNum + "的学生";
                    return;
                }
            }
            else
            {
                Std stemp = stds.Find(s => s.Name == GlobalInfo.AddScoreStdInfo);
                if (stemp != null)
                {
                    stemp.Score++;
                    Group grp = groups.Find(g => g.Leader.Name == GlobalInfo.AddScoreStdInfo);
                    if (grp != null)
                    {
                        grp.Score++;
                        GlobalInfo.AddScoreResult = "已为\n" + GlobalInfo.AddScoreStdInfo + "及其小组积分";
                        return;
                    }
                    foreach (var gr in groups)
                    {
                        stemp = gr.Members.Find(s => s.Name == GlobalInfo.AddScoreStdInfo);
                        if (stemp != null) { grp = groups.Find(g => g.Order == stemp.Group); break; }
                    }
                    if (grp != null) grp.Score++;
                    GlobalInfo.AddScoreResult = "已为\n" + GlobalInfo.AddScoreStdInfo + "及其小组积分";
                    return;
                }
                else
                {
                    GlobalInfo.AddScoreResult = "未找到\n姓名为" + GlobalInfo.AddScoreStdInfo + "的学生";
                    return;
                }
            }
        }

        private void AddScoreInfoBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                GlobalInfo.AddScoreStdInfo = AddScoreInfoBox.Text;
                AddStdWithInfo_Click(null, null);
            }
        }
    }
}
