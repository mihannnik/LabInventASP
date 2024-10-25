
using FileParser.Interfaces;
using FileParser.Models;
using System.Text.Json;

namespace FileParser.Services
{
	public class FileParserService(IRabbitMQService RabbitMQService) : IHostedService, IDisposable
	{
		CancellationTokenSource ShutDown = new CancellationTokenSource();
		string[] DeviceStates = ["Online", "Run", "NotReady", "Offline"];


		public Task StartAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			_ = Loop(ShutDown.Token);
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			ShutDown.Cancel();
            return Task.CompletedTask;
		}
		public async Task Loop(CancellationToken token)
		{
			string folder = Path.GetFullPath("xml");
			List<Models.DeviceStatus> PrevDeviceStatuses = new List<Models.DeviceStatus>();
			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
			while (true)
			{
				var files = Directory.GetFiles(folder, "*.xml");
				List<Models.DeviceStatus> DeviceStatuses = new List<Models.DeviceStatus>();
				List<Task> tasks = new List<Task>();
				foreach (var file in files)
				{
					tasks.Add(Task.Factory.StartNew(() =>
					{
						try
						{
							ReadOnlySpan<char> BaseSpan = File.ReadAllText(file).AsSpan();
							ReadOnlySpan<char> separator = "</DeviceStatus>".AsSpan();
							int StartIndex = BaseSpan.IndexOf(separator.Slice(2));
							ReadOnlySpan<char> Text = BaseSpan.Slice(StartIndex, BaseSpan.IndexOf("</InstrumentStatus>".AsSpan()) - StartIndex - 1);

							Span<Range> ranges = new Range[100];

							int numberOfSplits = Text.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries);
							ReadOnlySpan<char> ModulId = "ModuleCategoryID>".AsSpan();

							for (int i = 0; i < numberOfSplits; i++)
							{
								ReadOnlySpan<char> part = Text[ranges[i]];
								int PartStartIndex = part.IndexOf(ModulId);

								DeviceStatuses.Add(new Models.DeviceStatus
								{
									ModuleCategoryID = part.Slice(PartStartIndex + ModulId.Length, part.LastIndexOf(ModulId) - PartStartIndex - ModulId.Length - 2).ToString(),
									ModuleState = DeviceStates[Random.Shared.Next(0, 4)]
								});
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}));
				}
				await Task.WhenAll(tasks);
				if (PrevDeviceStatuses.Count > 0)
				{
					List<DeviceStatus> dif = PrevDeviceStatuses.Except(DeviceStatuses).ToList();
					Console.WriteLine($"DeviceID: {dif.First().ModuleCategoryID}, State: {dif.First().ModuleState}");
					RabbitMQService.SendMessage(dif);
				}
				else
				{
					RabbitMQService.SendMessage(DeviceStatuses);
				}
				PrevDeviceStatuses = DeviceStatuses;
				Thread.Sleep(1000);
			}
		}
		public void Dispose()
		{
			ShutDown.Dispose();
		}
	}
}
