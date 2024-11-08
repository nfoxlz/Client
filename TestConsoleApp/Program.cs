////DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
//DateTime startTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

//long timeStamp = 1698422400000L;

////var dateTime = DateTime.SpecifyKind(startTime.Add(new TimeSpan(timeStamp * 10000L)), DateTimeKind.Unspecified);
//var dateTime = startTime.Add(new TimeSpan(timeStamp * 10000L));


//Console.WriteLine(timeStamp);
//Console.WriteLine(dateTime);//Local:Error   Utc:Error
//Console.WriteLine(dateTime.ToLocalTime());//Local:Error Utc:OK
//Console.WriteLine(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime());//Local:OK     Utc:OK
//long result = (long)(dateTime - startTime).TotalMilliseconds;
//Console.WriteLine(result);
//if (timeStamp == result)
//    Console.WriteLine("OK");
//else
//    Console.WriteLine("Error");

//Console.WriteLine();
//var testDateTime = new DateTime(2023, 10, 28);
//Console.WriteLine(testDateTime);
//Console.WriteLine((long)(testDateTime - startTime).TotalMilliseconds);
//var testDateTime1 = TimeZoneInfo.ConvertTime(testDateTime, TimeZoneInfo.Utc);
//Console.WriteLine(testDateTime1);
//Console.WriteLine((long)(testDateTime1 - startTime).TotalMilliseconds);
//var testDateTime2 = TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(testDateTime, DateTimeKind.Local), TimeZoneInfo.Utc);
//Console.WriteLine(testDateTime2);
//Console.WriteLine((long)(testDateTime2 - startTime).TotalMilliseconds);

////Console.WriteLine(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
////Console.WriteLine(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeZoneInfo.Local));

bool? a = true;
bool b = false;
bool c = true;

bool f()
{
    return true;
}

Console.WriteLine((a ?? b) && f());

//System.Data.Common.DbCommand command = new System.Data.Common.DbCommand();
//command.ExecuteScalar