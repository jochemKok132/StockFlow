namespace StockFlow.UnitTests.Common
{
    public class ScheduleRulesTests
    {
        [Theory]
        [InlineData(2026, 11, 2)]
        [InlineData(2026, 11, 5)]
        [InlineData(2026, 11, 8)]
        public void WeekStart_AnyDayOfTheWeek_ReturnsItsMonday(int year, int month, int day)
        {
            Assert.Equal(new DateOnly(2026, 11, 2), ScheduleRules.WeekStart(new DateOnly(year, month, day)));
        }

        [Fact]
        public void UpcomingWeeks_ReturnsTheNextFourMondays()
        {
            var weeks = ScheduleRules.UpcomingWeeks(new DateOnly(2026, 10, 28)).ToList();

            Assert.Equal([new(2026, 11, 2), new(2026, 11, 9), new(2026, 11, 16), new DateOnly(2026, 11, 23)], weeks);
        }

        [Fact]
        public void Hours_StartAndEnd_ReturnsDecimalHours()
        {
            Assert.Equal(6.75m, ScheduleRules.Hours(new TimeOnly(14, 30), new TimeOnly(21, 15)));
        }

        [Theory]
        [InlineData(8, 21, true)]
        [InlineData(9, 21, false)]
        [InlineData(8, 20, false)]
        public void Covers_Window_ChecksTheWholeShift(int from, int until, bool expected)
        {
            Assert.Equal(expected, ScheduleRules.Covers(new TimeOnly(from, 0), new TimeOnly(until, 0), new TimeOnly(8, 0), new TimeOnly(21, 0)));
        }
    }
}
