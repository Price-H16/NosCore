﻿//  __  _  __    __   ___ __  ___ ___
// |  \| |/__\ /' _/ / _//__\| _ \ __|
// | | ' | \/ |`._`.| \_| \/ | v / _|
// |_|\__|\__/ |___/ \__/\__/|_|_\___|
// 
// Copyright (C) 2019 - NosCore
// 
// NosCore is a free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NosCore.Core.Configuration;
using NosCore.Core.HttpClients.ChannelHttpClients;
using NosCore.Core.I18N;
using NosCore.Data.Enumerations.I18N;
using NosCore.Database;
using NosCore.GameObject.Networking;
using Serilog;

namespace NosCore.LoginServer
{
    public class LoginServer
    {
        private readonly IChannelHttpClient _channelHttpClient;
        private readonly ILogger _logger;
        private readonly IOptions<LoginConfiguration> _loginConfiguration;
        private readonly NetworkManager _networkManager;
        private readonly NosCoreContext _context;

        public LoginServer(IOptions<LoginConfiguration> loginConfiguration, NetworkManager networkManager, ILogger logger, IChannelHttpClient channelHttpClient, NosCoreContext context)
        {
            _loginConfiguration = loginConfiguration;
            _networkManager = networkManager;
            _logger = logger;
            _channelHttpClient = channelHttpClient;
            _context = context;
        }

        public async Task RunAsync()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.Title += $@" - Port : {Convert.ToInt32(_loginConfiguration.Value.Port)}";
                }

                try
                {
                    await _context.Database.MigrateAsync();
                    await _context.Database.GetDbConnection().OpenAsync();
                    _logger.Information(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.DATABASE_INITIALIZED));
                }
                catch (Exception ex)
                {
                    _logger.Error("Database Error", ex);
                    _logger.Error(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.DATABASE_NOT_UPTODATE));
                    throw;
                }
                _logger.Information(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.LISTENING_PORT), _loginConfiguration.Value.Port);
                await Task.WhenAny(_channelHttpClient.ConnectAsync(), _networkManager.RunServerAsync()).ConfigureAwait(false);
            }
            catch
            {
                Console.ReadKey();
            }
        }
    }
}