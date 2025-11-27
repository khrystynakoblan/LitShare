// <copyright file="AppLogger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.BLL.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Static logger for the LitShare application.
    /// Uses Serilog and Microsoft.Extensions.Logging to log information, warnings, and errors.
    /// </summary>
    public static class AppLogger
    {
        private static Microsoft.Extensions.Logging.ILogger logger = null!;

        /// <summary>
        /// Ініціалізація Logger (викликається один раз при старті програми).
        /// </summary>
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Мінімальний рівень для Serilog
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")

                .WriteTo.File(
                    path: "Logs/log-.txt",  // Щоденні файли
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            // Інтегруємо Serilog у Microsoft ILogger
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            logger = factory.CreateLogger("LitShareApp");
        }

        /// <summary>
        /// Лог попередження (Warning).
        /// </summary>
        /// <param name="message">Повідомлення для логування.</param>
        public static void Info(string message)
        {
            logger.LogInformation(message);
        }

        /// <summary>
        /// Лог попередження (Warning).
        /// </summary>
        /// <param name="message">Повідомлення, яке потрібно зафіксувати як попередження.</param>
        public static void Warn(string message)
        {
            logger.LogWarning(message);
        }

        /// <summary>
        /// Лог помилки (Error).
        /// </summary>
        /// <param name="message">Повідомлення про помилку для логування.</param>
        /// <param name="ex">Об'єкт виключення, яке стало причиною помилки (необов'язково).</param>
        public static void Error(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                logger.LogError(ex, message);
            }
            else
            {
                logger.LogError(message);
            }
        }
    }
}
