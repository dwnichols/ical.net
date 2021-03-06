using System;
using System.Linq;
using Ical.Net.DataTypes;
using Ical.Net.Serialization.iCalendar.Serializers.DataTypes;
using NUnit.Framework;

namespace Ical.Net.UnitTests.Serialization.iCalendar.Serializers.DataTypes
{
    [TestFixture]
    public class DateTimeSerializerTests
    {
        [Test, Category("Deserialization")]
        public void TZIDPropertyMustNotBeAppliedToUtcDateTime()
        {
            var ical = new Ical.Net.Calendar();
            var evt = new Ical.Net.CalendarEvent();
            evt.DtStamp = new CalDateTime(new DateTime(2016, 8, 17, 2, 30, 0, DateTimeKind.Utc));
            ical.Events.Add(evt);

            var serializer = new Ical.Net.Serialization.iCalendar.Serializers.CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(ical);

            var lines = serializedCalendar.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var result = lines.First(s => s.StartsWith("DTSTAMP"));
            Assert.AreEqual("DTSTAMP:20160817T023000Z", result);
        }

        [Test, Category("Deserialization")]
        public void TZIDPropertyShouldBeAppliedForLocalTimezones()
        {
            // see http://www.ietf.org/rfc/rfc2445.txt p.36
            var result = DateTimeSerializer()
                .SerializeToString(
                new CalDateTime(new DateTime(1997, 7, 14, 13, 30, 0, DateTimeKind.Local), "US-Eastern"));

            // TZID is applied elsewhere - just make sure this doesn't have 'Z' appended. 
            Assert.AreEqual("19970714T133000", result);
        }


        private static DateTimeSerializer DateTimeSerializer()
        {
            return new DateTimeSerializer();
        }
    }
}