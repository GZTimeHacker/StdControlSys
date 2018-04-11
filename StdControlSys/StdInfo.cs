using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Windows;
using System.Threading;
using System.ComponentModel;

namespace StdControlSys
{
    #region 基本数据类
    /// <summary>
    /// 学生基本信息对象
    /// </summary>
    public class Std
    {
        /// <summary>
        /// 班级
        /// </summary>
        public int Class { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 学生积分
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 所在小组
        /// </summary>
        public int Group { get; set; }
        /// <summary>
        /// 新建学生对象
        /// </summary>
        public Std()
        {
            Class = 0;
            Name = "null";
            Number = 0;
            Score = 0;
        }
        /// <summary>
        /// 新建学生对象
        /// </summary>
        /// <param name="Class">班级</param>
        /// <param name="Name">姓名</param>
        /// <param name="Number">学号</param>
        public Std(int Class,string Name,int Number)
        {
            this.Class = Class;
            this.Name = Name;
            this.Number = Number;
            Score = 0;
        }
        public override string ToString()
        {
            return "班级：高一(" + Class + ")班" +
                "\n学号：" + Number +
                "\n姓名：" + Name;
        }
    }

    /// <summary>
    /// 小组对象
    /// </summary>
    public class Group : INotifyPropertyChanged
    {
        /// <summary>
        /// 组员
        /// </summary>
        public List<Std> Members;
        /// <summary>
        /// 组长
        /// </summary>
        public Std Leader;
        /// <summary>
        /// 组名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 小组编号
        /// </summary>
        public int Order { get; set; }
        private int _Score;
        /// <summary>
        /// 小组积分
        /// </summary>
        public int Score
        {
            get { return _Score; }
            set
            {
                _Score = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Score"));
            }
        }
        /// <summary>
        /// 实例化小组
        /// </summary>
        /// <param name="stds">学生列表</param>
        /// <param name="info">小组信息</param>
        public Group(List<Std> stds,GroupInfo info)
        {
            Members = new List<Std>();
            Name = info.Name;
            Order = info.Order;
            Leader = stds.Find(s => s.Number == info.LeaderNum);
            Leader.Group = Order;
            foreach (int n in info.MembersNum)
            {
                Members.Add(stds.Find(s => s.Number == n));
            }
            foreach (Std i in Members)
            {
                i.Group = Order;
            }
            Score = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            string str="";
            str += Name + "\t[" + Order + "]\n";
            str += "组长:" + Leader.Name + "\n组员:\n";
            foreach (Std s in Members)
            {
                str += s.Name + " ";
            }
            str += "\n";
            return str;
        }
    }

    /// <summary>
    /// 小组信息对象
    /// </summary>
    public class GroupInfo
    {
        /// <summary>
        /// 小组名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 小组序号
        /// </summary>
        public int Order;
        /// <summary>
        /// 组长学号
        /// </summary>
        public int LeaderNum;
        /// <summary>
        /// 组员学号
        /// </summary>
        public int[] MembersNum;
        /// <summary>
        /// 新建小组信息对象
        /// </summary>
        public GroupInfo()
        {
            Name = "";
            Order = -1;
            LeaderNum = 0;
            MembersNum = null;
        }
        /// <summary>
        /// 新建小组信息对象
        /// </summary>
        /// <param name="Name">小组名称</param>
        /// <param name="Order">小组序号</param>
        /// <param name="LeaderNum">组长学号</param>
        /// <param name="MembersNum">组员学号数组</param>
        public GroupInfo(string Name,int Order, int LeaderNum,int[] MembersNum)
        {
            this.Name = Name;
            this.Order = Order;
            this.LeaderNum = LeaderNum;
            this.MembersNum = MembersNum;
        }
        public override string ToString()
        {
            string str = "";
            str += Name + "\t[" + Order + "]\n";
            str += "组长:" + LeaderNum+ "\n组员:";
            foreach (int s in MembersNum)
            {
                str += s + "  ";
            }
            str += "\n";
            return str;
        }
    }

    /// <summary>
    /// 读取与写入学生和班级信息
    /// </summary>
    public class Info
    {
        /// <summary>
        /// 序列化StdInfo数组
        /// </summary>
        /// <param name="stds">学生信息数组</param>
        /// <param name="FileName">文件位置</param>
        /// <returns>序列化是否成功</returns>
        static public bool SerializerStd(Std[] stds,string FileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Std[]));
            using(FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    xmlSerializer.Serialize(fs, stds);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
        /// <summary>
        /// 序列化StdInfo数组(List)
        /// </summary>
        /// <param name="stds">学生信息数组</param>
        /// <param name="FileName">文件位置</param>
        /// <returns>序列化是否成功</returns>
        static public bool SerializerStd(List<Std> stds, string FileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Std>));
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    xmlSerializer.Serialize(fs, stds);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
        /// <summary>
        /// 反序列化StdInfo数组
        /// </summary>
        /// <returns>学生信息数组</returns>
        static public Std[] DeserializeStd(string FileName)
        {
            Std[] stds;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Std[]));
            using(FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    stds = (Std[])xmlSerializer.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"错误信息",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
            return null;
        }
        /// <summary>
        /// 反序列化StdInfo数组(List)
        /// </summary>
        /// <returns>学生信息列表</returns>
        static public List<Std> DeserializeStd_List(string FileName)
        {
            List<Std> stds;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Std>));
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    stds = (List<Std>)xmlSerializer.Deserialize(fs);
                    return stds;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return null;
        }
        /// <summary>
        /// 序列化小组信息数组(List)
        /// </summary>
        /// <param name="groups">小组信息数组</param>
        /// <param name="FileName">文件位置</param>
        /// <returns>序列化是否成功</returns>
        static public bool SerializerGrp(List<GroupInfo> groups,string FileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<GroupInfo>));
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    xmlSerializer.Serialize(fs, groups);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
        /// <summary>
        /// 反序列化小组信息数组(List)
        /// </summary>
        /// <param name="FileName">文件位置</param>
        /// <returns>小组信息列表</returns>
        static public List<GroupInfo> DeserializeGrp_List(string FileName)
        {
            List<GroupInfo> groups;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<GroupInfo>));
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    groups = (List<GroupInfo>)xmlSerializer.Deserialize(fs);
                    return groups;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return null;
        }
        /// <summary>
        /// 序列化积分信息
        /// </summary>
        /// <param name="score">积分信息</param>
        /// <param name="FileName">文件位置</param>
        /// <returns>序列化是否成功</returns>
        static public bool SerializerScore(ScoreData score,string FileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScoreData));
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    xmlSerializer.Serialize(fs, score);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
        /// <summary>
        /// 反序列化积分信息
        /// </summary>
        /// <param name="FileName">文件位置</param>
        /// <returns>积分信息</returns>
        static public ScoreData DeserializeScore(string FileName)
        {
            ScoreData score;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScoreData));
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                try
                {
                    score = (ScoreData)xmlSerializer.Deserialize(fs);
                    return score;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return null;
        }
    }
    #endregion

    /// <summary>
    /// 全局变量
    /// </summary>
    class Global : INotifyPropertyChanged
    {
        private string _SelectedGroupInfo = "waiting...";
        /// <summary>
        /// 被选择的小组信息
        /// </summary>
        public string SelectedGroupInfo
        {
            get { return _SelectedGroupInfo; }
            set { _SelectedGroupInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGroupInfo"));
            }
        }

        private string _SelectedStdInfo = "waiting...";
        /// <summary>
        /// 被选择的学生信息
        /// </summary>
        public string SelectedStdInfo
        {
            get { return _SelectedStdInfo; }
            set
            {
                _SelectedStdInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStdInfo"));
            }
        }

        private string _FlowerTokenChar = "完";
        /// <summary>
        /// 飞花令字符
        /// </summary>
        public string FlowerTokenChar
        {
            get { return _FlowerTokenChar; }
            set
            {
                _FlowerTokenChar = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlowerTokenChar"));
            }
        }

        private string _SelectedStdInGrpInfo = "waiting...";
        /// <summary>
        /// 小组中抽取的学生信息
        /// </summary>
        public string SelectedStdInGrpInfo
        {
            get { return _SelectedStdInGrpInfo; }
            set
            {
                _SelectedStdInGrpInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStdInGrpInfo"));
            }
        }

        private bool _IsAutoStdFromGroup = true;
        /// <summary>
        /// 是否自动抽取组内成员
        /// </summary>
        public bool IsAutoStdFromGroup
        {
            get { return _IsAutoStdFromGroup; }
            set
            {
                _IsAutoStdFromGroup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAutoStdFromGroup"));
            }
        }

        private string _AddScoreStdInfo = "170000";
        /// <summary>
        /// 需要加分同学信息
        /// </summary>
        public string AddScoreStdInfo
        {
            get { return _AddScoreStdInfo; }
            set
            {
                _AddScoreStdInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddScoreStdInfo"));
            }
        }

        private string _AddScoreResult = "";
        /// <summary>
        /// 加分结果
        /// </summary>
        public string AddScoreResult
        {
            get { return _AddScoreResult; }
            set
            {
                _AddScoreResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddScoreResult"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ScoreData
    {
        /// <summary>
        /// 小组信息MD5
        /// </summary>
        public string GroupInfoMD5 { get; set; }
        /// <summary>
        /// 学生信息MD5
        /// </summary>
        public string StdDataMD5 { get; set; }
        /// <summary>
        /// 小组得分
        /// </summary>
        public List<int> GroupScore { get; set; }
        /// <summary>
        /// 学生得分
        /// </summary>
        public List<int> StudentScore { get; set; }
        public ScoreData()
        {
            GroupInfoMD5 = "00000000";
            StdDataMD5 = "00000000";
            GroupScore = null;
            StudentScore = null;
        }
        /// <summary>
        /// 新建成绩信息
        /// </summary>
        /// <param name="GroupMD5">小组信息文件MD5</param>
        /// <param name="StdMD5">学生数据文件MD5</param>
        /// <param name="GrpScore">小组积分列表</param>
        /// <param name="StdScore">学生积分列表</param>
        public ScoreData(string GroupMD5,string StdMD5,List<int> GrpScore,List<int> StdScore)
        {
            GroupInfoMD5 = GroupMD5;
            StdDataMD5 = StdMD5;
            GroupScore = GrpScore;
            StudentScore = StdScore;
        }
    }
}
