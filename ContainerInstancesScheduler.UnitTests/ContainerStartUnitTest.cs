using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NCrontab;
using Xunit;
using OperationsApps;
using Moq;
using OperationApps.Util;
using ContainerScheduleDetail = OperationsApps.ContainerScheduleDetail;

namespace ContainerInstancesScheduler.UnitTests
{
    public class ContainerStartUnitTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Theory]
        [InlineData("2020-06-09 12:00:00", "2020-06-09 12:16:00")]
        [InlineData("2020-06-09 12:01:00", "2020-06-09 12:16:00")]
        [InlineData("2020-06-09 12:05:00", "2020-06-09 12:16:00")]
        [InlineData("2020-06-09 12:10:00", "2020-06-09 12:45:00")]
        public void TestCronExpressionWithinRange(string startDate , string endDate)
        {
            var cronSchedule = CrontabSchedule.Parse("15 12 * * *");
            var startDateTime = DateTime.Parse(startDate);
            var endDateTime = DateTime.Parse(endDate);
            var occurrence =
                cronSchedule.GetNextOccurrence(startDateTime , endDateTime);
             Assert.True(occurrence.Equals(DateTime.Parse("2020-06-09 12:15:00")));
             var occurrences =
                 cronSchedule.GetNextOccurrences(startDateTime, endDateTime);
             Assert.Single(occurrences);

        }

        [Theory]
        [InlineData("2020-06-09 12:00:00", "2020-06-09 12:15:00", "15 12 * * *")]
        [InlineData("2020-06-09 12:01:00", "2020-06-09 12:15:00", "15 12 * * *")]
        [InlineData("2020-06-09 12:15:00", "2020-06-09 12:16:00", "15 12 * * *")]
        [InlineData("2020-06-09 12:15:00", "2020-06-09 12:45:00", "15 12 * * *")]
        public void TestCronExpressionInRange(string startDate, string endDate, string scheduleDate)
        {

            var cronSchedule = CrontabSchedule.Parse(scheduleDate);
            var startDateTime = DateTime.Parse(startDate);
            var endDateTime = DateTime.Parse(endDate);

            var occurrences =
                cronSchedule.GetNextOccurrences(startDateTime, endDateTime);
            Assert.False(occurrences.Any());
        }


        [Theory]
        [InlineData("2020-06-09 12:00:00", "2020-06-09 12:15:00", "00 15 * * *")]
        [InlineData("2020-06-09 12:01:00", "2020-06-09 12:15:00", "00 15 * * *")]
        [InlineData("2020-06-09 12:05:00", "2020-06-09 12:15:00", "00 15 * * *")]
        [InlineData("2020-06-09 12:10:00", "2020-06-09 12:15:00", "30 12 * * *")]
        [InlineData("2020-06-09 12:10:00", "2020-06-09 12:15:00", "00 20 * * *")]
        public void TestCronExpressionExpiredRange(string startDate, string endDate, string scheduleDate)
        {
            var cronSchedule = CrontabSchedule.Parse(scheduleDate);
            var startDateTime = DateTime.Parse(startDate);
            var endDateTime = DateTime.Parse(endDate);
            var occurence =
                cronSchedule.GetNextOccurrences(startDateTime, endDateTime);
            Assert.False(occurence.Any());
        }


        [Theory]
        [InlineData("2020-06-09 12:00:00", "2020-06-09 12:15:00", "00 11 * * *")]
        [InlineData("2020-06-09 12:01:00", "2020-06-09 12:15:00", "00 11 * * *")]
        [InlineData("2020-06-09 12:05:00", "2020-06-09 12:15:00", "00 11 * * *")]
        [InlineData("2020-06-09 12:10:00", "2020-06-09 12:15:00", "00 11 * * *")]
        [InlineData("2020-06-09 12:10:00", "2020-06-09 12:15:00", "00 20 * * *")]
        
        public void TestCronExpressionOutsideInFutureRange(string startDate, string endDate, string scheduleDate)
        {
            var cronSchedule = CrontabSchedule.Parse(scheduleDate);
            var startDateTime = DateTime.Parse(startDate);
            var endDateTime = DateTime.Parse(endDate);
            var occurence =
                cronSchedule.GetNextOccurrences(startDateTime, endDateTime);
            Assert.False(occurence.Any());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-2)]
        public void ContainersStart_Should_Start_OnScheduleTime(int addMinutes)
        {
            var logger = (ListLogger) TestFactory.CreateLogger(LoggerTypes.List);
            var executionContext = new ExecutionContext
            {
                FunctionAppDirectory = ""
            };
            var testDateTime = DateTime.Now.AddMinutes(addMinutes);
            var containerScheduleDetail = new ContainerScheduleDetail
            { 
                ResourceGroupName = "",
                ContainerImageName = "",
                StartSchedule = $"{testDateTime.Minute} {testDateTime.Hour} * * *"
            };
            ContainersStart containersStart = new ContainersStart(GetMockTableStorageUtility(containerScheduleDetail), GetAzureContainerUtility());
            containersStart.Run(null, executionContext, logger);

            var msg = logger.Logs[0];
            var containerImagesStartedMsg = logger.Logs[2];
            var containerImagesEndMsg = logger.Logs[3];
            Assert.Contains("ContainersStart function executed at", msg);
            Assert.Contains("ContainersStart function Started", containerImagesStartedMsg);
            Assert.Contains("ContainersStart function completed", containerImagesEndMsg);

        }

        [Theory]
        [InlineData(20)]
        [InlineData(-20)]
        [InlineData(-16)]
        [InlineData(16)]
        public void ContainersStart_Should_Start_Expired_FutureTime(int addMinutes)
        {
            var logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            var executionContext = new ExecutionContext
            {
                FunctionAppDirectory = ""
            };
            var testDateTime = DateTime.Now.AddMinutes(-20);
            var containerScheduleDetail = new ContainerScheduleDetail
            {
                ResourceGroupName = "",
                ContainerImageName = "",
                StartSchedule = $"{testDateTime.Minute} {testDateTime.Hour} * * *"
            };

            var containersStart = new ContainersStart(GetMockTableStorageUtility(containerScheduleDetail), GetAzureContainerUtility());
            containersStart.Run(null, executionContext, logger);

            var msg = logger.Logs[0];
            var containerImagesMsg = logger.Logs[3];

            Assert.Contains("ContainersStart function executed at", msg);
            Assert.Contains("ContainersStart function Completed", containerImagesMsg);
            Assert.DoesNotContain("ContainersStart function Started", containerImagesMsg);


        }

        private ITableStorageUtility GetMockTableStorageUtility(ContainerScheduleDetail containerScheduleDetail)
        {

            var mockTableStorageUtility = new Mock<ITableStorageUtility>();
            _ = mockTableStorageUtility.Setup(tableStorageUtility => tableStorageUtility
                  .GetContainerScheduleDetails<ContainerScheduleDetail>()).Returns(async () =>
              {
                  var containerScheduleDetails = new List<ContainerScheduleDetail> {containerScheduleDetail};
                  return containerScheduleDetails;
              });

            return mockTableStorageUtility.Object;
        }

        private IAzureContainerUtility GetAzureContainerUtility()
        {
            var azureContainerUtility = new Mock<IAzureContainerUtility>();
            azureContainerUtility.Setup(moc => moc.StartContainer("", "", ""))
                .Returns(() => true);
            return azureContainerUtility.Object;
        }
    }
}
