using Fleksitid.Models;

namespace Fleksitid.Services
{
    /// <summary>
    /// Regner ut fleksisaldo: sum arbeidede timer minus forventede timer basert på kontrakt,
    /// der sykedager telles som at forventet arbeidstid er oppfylt (verken pluss eller minus).
    /// </summary>
    public static class FlexTimeCalculator
    {
        public static decimal CalculateBalance(
            IEnumerable<WorkContract> contracts,
            IEnumerable<TimeEntry> entries,
            IEnumerable<SickDay> sickDays,
            DateOnly asOf)
        {
            var contractList = contracts.OrderBy(c => c.StartDate).ToList();
            if (contractList.Count == 0)
            {
                return 0m;
            }

            var sickDates = new HashSet<DateOnly>(
                sickDays.SelectMany(s => DatesBetween(s.FromDate, s.ToDate)));

            var worked = entries.Sum(e => e.Hours);

            decimal expected = 0m;
            var earliestStart = contractList.Min(c => c.StartDate);
            for (var date = earliestStart; date <= asOf; date = date.AddDays(1))
            {
                if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    continue;
                }

                if (sickDates.Contains(date))
                {
                    continue;
                }

                var contract = contractList.LastOrDefault(c => c.IsActiveOn(date));
                if (contract is not null)
                {
                    expected += contract.WeeklyHours / 5m;
                }
            }

            return worked - expected;
        }

        private static IEnumerable<DateOnly> DatesBetween(DateOnly from, DateOnly to)
        {
            for (var date = from; date <= to; date = date.AddDays(1))
            {
                yield return date;
            }
        }
    }
}
