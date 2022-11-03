using System;
using System.Collections.Generic;
using UsageHelper;

namespace WebAPI.Utils
{
    public class ReportHandler
    {
        public ReportHandler()
        {

        }

        public double CalculationTime(TimeSpan timeOfDayStart, TimeSpan timeOfDayEnd)
        {
            TimeSpan inTime = new TimeSpan(Const.TIME_BEGIN_HOUR, Const.TIME_BEGIN_MINUTE, 0);
            TimeSpan outTime = new TimeSpan(Const.TIME_END_HOUR, Const.TIME_END_MINUTE, 0);
            TimeSpan breakTimeStart = new TimeSpan(12, 0, 0);
            TimeSpan breakTimeEnd = new TimeSpan(13, 0, 0);
            TimeSpan workTime = new TimeSpan();
            double totalTime = 0;
            if (timeOfDayEnd > timeOfDayStart)
            {
                TimeSpan checkIn = new TimeSpan(0, 0, 0);
                TimeSpan checkOut = new TimeSpan(0, 0, 0);
                TimeSpan breakTime = new TimeSpan(0, 0, 0);
                if (timeOfDayStart < inTime) //<8h
                {
                    checkIn = inTime;
                }
                else// >8h
                {
                    if (timeOfDayStart <= breakTimeStart)// <12h
                    {
                        checkIn = timeOfDayStart;
                    }
                    else // >12h
                    {
                        if (timeOfDayStart < breakTimeEnd) // <13h
                        {
                            checkIn = breakTimeEnd;
                        }
                        else //>13h
                        {
                            if (timeOfDayStart <= outTime) //<17h
                            {
                                checkIn = timeOfDayStart;
                            }
                            else // >17h
                            {
                                checkIn = outTime;
                            }
                        }
                    }
                }

                if (timeOfDayEnd > checkOut)//>17h
                {
                    checkOut = outTime;
                }//<17h
                {
                    if (timeOfDayEnd >= breakTimeEnd)//>13h
                    {
                        checkOut = timeOfDayEnd;
                    }
                    else //<13h
                    {
                        if (timeOfDayEnd > breakTimeStart)//>12h
                        {
                            checkOut = breakTimeStart;
                        }    //<12h
                        else
                        {
                            if (timeOfDayEnd >= inTime)//>8h
                            {
                                checkOut = timeOfDayEnd;
                            }
                            else//<8h
                            {
                                checkOut = inTime;
                            }
                        }
                    }
                }
                //
                if (timeOfDayStart <= breakTimeStart && timeOfDayEnd >= breakTimeEnd)
                {
                    breakTime = new TimeSpan(1, 0, 0);
                }
                workTime = checkOut - checkIn - breakTime;
                totalTime = workTime.TotalHours;
            }
            return totalTime;
        }
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from; day.Date <= to.Date; day = day.AddDays(1))
            {
                if (day.Date == to.Date)
                {
                    yield return to;
                }
                else
                {
                    yield return day;
                }
            }
        }
    }
}
