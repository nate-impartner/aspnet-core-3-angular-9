using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck
{
	public class ICMPHealthCheck : IHealthCheck
	{
		public string Host { get; private set; }
		public int Timeout { get; private set; }

		public ICMPHealthCheck(string host, int timeOut)
		{
			Host = host;
			Timeout = timeOut;
		}

		#region Implementation of IHealthCheck

		/// <inheritdoc />
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			try
			{
				using var ping = new Ping();
				var reply = await ping.SendPingAsync(Host);

				switch(reply.Status)
				{
					case IPStatus.Success:
					{
						var msg = $"IMCP to {Host} took {reply.RoundtripTime} ms";
						return (reply.RoundtripTime > Timeout)
							? HealthCheckResult.Degraded(msg)
							: HealthCheckResult.Healthy(msg);
					}
					default:
					{
						var err = $"IMCP to {Host} failed: {reply.Status}";
						return HealthCheckResult.Unhealthy(err);
					}
				}
			}
			catch (Exception e)
			{
				var err = $"IMCP to {Host} failed: {e.Message}";
				return HealthCheckResult.Unhealthy(err);
			}
		}

		#endregion
	}
}