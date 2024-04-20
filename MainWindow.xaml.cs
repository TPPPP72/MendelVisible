using System.Data;
using System.Windows;

namespace MendelVisible
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Num.Value = 1;
            Precise.Value = 1;
        }

        private static List<string> SplitByLen(string str, int separatorCharNum)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= separatorCharNum)
            {
                return [str];
            }
            string tempStr = str;
            List<string> strList = [];
            int iMax = Convert.ToInt32(Math.Ceiling(str.Length / (separatorCharNum * 1.0)));
            for (int i = 1; i <= iMax; i++)
            {
                string currMsg = tempStr[..(tempStr.Length > separatorCharNum ? separatorCharNum : tempStr.Length)];
                strList.Add(currMsg);
                if (tempStr.Length > separatorCharNum)
                {
                    tempStr = tempStr[separatorCharNum..];
                }
            }
            return strList;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            List<string> res1 = SplitByLen(Data1.Text, 2), res2 = SplitByLen(Data2.Text, 2);
            List<string> Dis1 = [], Dis2 = [];
            if (Data3.Text.Length > 0)
            {
                string[] com = Data3.Text.Split(',');
                foreach (var str in com)
                {
                    if (str[0] == '1')
                        Dis1.Add(str[1..]);
                    else if (str[0] == '2')
                        Dis2.Add(str[1..]);
                    else
                    {
                        Dis1.Add(str);
                        Dis2.Add(str);
                    }
                }
            }
            var random = new Random();
            Dictionary<string, int> map = [];
            int time = 0;
            while (time < Num.Value)
            {
                string part1 = string.Empty, part2 = string.Empty;
                foreach (var s in res1)
                    part1 += s[random.Next(2)];
                while (Dis1.Exists(t => t == part1))
                {
                    part1 = string.Empty;
                    foreach (var s in res1)
                        part1 += s[random.Next(2)];
                }
                foreach (var s in res2)
                    part2 += s[random.Next(2)];
                while (Dis2.Exists(t => t == part2))
                {
                    part2 = string.Empty;
                    foreach (var s in res2)
                        part2 += s[random.Next(2)];
                }
                string result = string.Empty;
                for (int i = 0; i < part1.Length; i++)
                {
                    if (part1[i] > part2[i])
                        result = result + part2[i] + part1[i];
                    else
                        result = result + part1[i] + part2[i];
                }
                time++;
                if (map.TryGetValue(result, out int value))
                    map[result] = ++value;
                else
                {
                    if (result.Length > 0)
                        map.Add(result, 1);
                }
            }
            if (map.Count > 0)
            {
                Table1.ItemsSource = map;
                Table1.Columns[0].Header = "基因型";
                Table1.Columns[1].Header = "数量";
            }
            Dictionary<string, int> amap = [];
            foreach (KeyValuePair<string, int> pair in map)
            {
                List<string> res = SplitByLen(pair.Key, 2);
                string result= string.Empty;
                foreach (string s in res)
                {
                    if (s.Contains(s.ToUpper()[0]))
                        result = result + s.ToUpper()[0] + '_';
                    else
                        result = result + s.ToLower()[0] + s.ToLower()[0];
                }
                if(amap.TryGetValue(result,out int value))
                    amap[result] = value+pair.Value;
                else
                    amap.Add(result, pair.Value);
            }
            if(amap.Count > 0)
            {
                DataTable dt = new();
                dt.Columns.Add("表型", typeof(string));
                dt.Columns.Add("数量", typeof(int));
                dt.Columns.Add("比例", typeof(string));
                DataRow dr;
                int min = int.MaxValue;
                foreach(var item in amap)
                    min=Math.Min(min, item.Value);
                foreach(var item in amap)
                {
                    dr = dt.NewRow();
                    dr["表型"] = item.Key;
                    dr["数量"]=item.Value;
                    string precise = "0.";
                    for (int i = 0; i < Precise.Value; i++)
                        precise += '0';
                    dr["比例"] =Convert.ToDouble(((double)item.Value / min)).ToString(precise);
                    dt.Rows.Add(dr);
                }
                Table2.DataContext = dt;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Table1.ItemsSource = null;
            Table2.ItemsSource = null;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
