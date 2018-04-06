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
        #region 后台启动工作
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectedGroup.DataContext = GlobalInfo;
            this.SelectedStd.DataContext = GlobalInfo;
            this.FlowerTokenWord.DataContext = GlobalInfo;
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
                GlobalInfo.FlowerTokenChar = CreateCode(1);
                Thread.Sleep(20 + i * i / 2 );
            }
            char Selected = FlowersWords.ElementAtOrDefault(random.Next(0, FlowersWords.Length - 1));
            GlobalInfo.FlowerTokenChar = Selected.ToString();
            FlowersWords = FlowersWords.Remove(FlowersWords.IndexOf(Selected), 1);
        }
        /// <summary>
        /// 随机生成汉字
        /// </summary>
        /// <param name="strlength">长度</param>
        /// <returns></returns>
        public string CreateCode(int strlength)
        {
            //定义一个字符串数组储存汉字编码的组成元素 
            string[] r = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            Random rnd = new Random();
            //定义一个object数组用来 
            object[] bytes = new object[strlength];
            /**/
            /*每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bject数组中
             * */
            for (int i = 0; i < strlength; i++)
            {
                //区位码第1位 
                int r1 = rnd.Next(11, 14);
                string str_r1 = r[r1].Trim();
                //区位码第2位 
                rnd = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i);//更换随机数发生器的种子避免产生重复值 
                int r2;
                if (r1 == 13)
                    r2 = rnd.Next(0, 7);
                else
                    r2 = rnd.Next(0, 16);
                string str_r2 = r[r2].Trim();
                //区位码第3位 
                rnd = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
                int r3 = rnd.Next(10, 16);
                string str_r3 = r[r3].Trim();
                //区位码第4位 
                rnd = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
                int r4;
                if (r3 == 10)
                {
                    r4 = rnd.Next(1, 16);
                }
                else if (r3 == 15)
                {
                    r4 = rnd.Next(0, 15);
                }
                else
                {
                    r4 = rnd.Next(0, 16);
                }
                string str_r4 = r[r4].Trim();
                //定义两个字节变量存储产生的随机汉字区位码 
                byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
                byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
                //将两个字节变量存储在字节数组中 
                byte[] str_r = new byte[] { byte1, byte2 };
                //将产生的一个汉字的字节数组放入object数组中 
                bytes.SetValue(str_r, i);
            }
            //获取GB2312编码页（表） 
            Encoding gb = Encoding.GetEncoding("gb2312");
            //根据汉字编码的字节数组解码出中文汉字 
            string str1 = gb.GetString((byte[])Convert.ChangeType(bytes[0], typeof(byte[])));
           // string str2 = gb.GetString((byte[])Convert.ChangeType(bytes[1], typeof(byte[])));
            //string str3 = gb.GetString((byte[])Convert.ChangeType(bytes[2], typeof(byte[])));
            //string str4 = gb.GetString((byte[])Convert.ChangeType(bytes[3], typeof(byte[])));
            //string txt = str1 + str2 + str3 + str4;
            return str1;
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
                GlobalInfo.SelectedStdInfo = std.Name + "\n" + std.Number;
                Thread.Sleep(20 + i * i / 2);
            }
        }
        #endregion
    }
}
