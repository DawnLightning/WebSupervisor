using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
namespace WebSupervisor.Code.Classes
{
    public class CalendarTools
    {
        //算周次，以星期一为一周的开始
        public static int WeekOfYear(int y, int m, int d)
        {
            DateTime dt = new DateTime(y, m, d);
            DayOfWeek dw = (Convert.ToDateTime(string.Format("{0}-1-1 0:0:0", dt.Year.ToString()))).DayOfWeek;
            int day = 0;
            switch (dw)
            {

                case DayOfWeek.Monday:
                    {
                        day = -1;
                        break;
                    }
                case DayOfWeek.Tuesday:
                    {
                        day = 0;
                        break;
                    }
                case DayOfWeek.Wednesday:
                    {
                        day = 1;
                        break;
                    }
                case DayOfWeek.Thursday:
                    {
                        day = 2;
                        break;
                    }
                case DayOfWeek.Friday:
                    {
                        day = 3;
                        break;
                    }
                case DayOfWeek.Saturday:
                    {
                        day = 4;
                        break;
                    }
                case DayOfWeek.Sunday:
                    {
                        day = 5;
                        break;
                    }
            }
            int week = (dt.DayOfYear + day) / 7 + 1;

            return week;
        }

        //算星期
        public static string CaculateWeekDay(int y, int m, int d)
        {
            if (m == 1 || m == 2)
            {
                m = 12 + m;
                y = y - 1;
            }
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7 + 1;
            string weekstr = "";
            switch (week)
            {
                case 1: weekstr = "一"; break;
                case 2: weekstr = "二"; break;
                case 3: weekstr = "三"; break;
                case 4: weekstr = "四"; break;
                case 5: weekstr = "五"; break;
                case 6: weekstr = "六"; break;
                case 7: weekstr = "日"; break;
            }
            return weekstr;
        }


        //算日期
        public static DateTime getdata(int n, int weeks, int weekday, int y, int r)//年，周次，星期，月，日
        {
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            DateTime starttime = new DateTime(n, y, r);//开学日期
            int days;//根据周次，星期算出天数差
            days = (weeks - 1) * 7 + weekday;//天数差
            DateTime thisdate = calendar.AddDays(starttime, days);//所求日期
            return thisdate;
        }

        //将大写的星期转化成数字
        public static int weekdays(string xingqi)
        {

            int weekday = 0;
            switch (xingqi)
            {

                case "一": weekday = 1; break;
                case "二": weekday = 2; break;
                case "三": weekday = 3; break;
                case "四": weekday = 4; break;
                case "五": weekday = 5; break;
                case "六": weekday = 6; break;
                case "日": weekday = 7; break;
            }
            return weekday;
        }
    }
}

