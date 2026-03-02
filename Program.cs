using System;
using System.IO;
using System.Windows.Forms;
using FBZ_System.Repositories;
using FBZ_System.Services;
using FBZ_System.Strategies;

namespace FBZ_System
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // repository
            var repository = new ComicRepositoryCsv(AppDomain.CurrentDomain.BaseDirectory + "Data");

            // strategies
            var groupingStrategies = new List<IGroupingStrategy>
    {
        new GroupByAuthorStrategy(),
        new GroupByYearStrategy()
    };

            var sortStrategies = new List<ISortStrategy>
    {
        new SortTitleAscendingStrategy(),
        new SortTitleDescendingStrategy()
    };

            // services
            var searchService = new SearchService(repository, groupingStrategies, sortStrategies);
            var history = new SearchHistoryService();
            var formatter = new ComicFormatter();

            // start the UI
            Application.Run(new Form1(repository, searchService, history, formatter));
        }
        }
    }
