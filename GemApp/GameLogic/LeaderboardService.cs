
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GemApp.Models;
using Microsoft.Maui.Storage;

namespace GemApp.GemLogic;

public class LeaderboardService
{
    private readonly string filePath =
        Path.Combine(
            FileSystem.AppDataDirectory,
            "gemapp_leaderboard.json");

    // EDETABELI LAADIMINE
    public async Task<List<LeaderboardEntry>> LoadAsync()
    {
        try
        {
            if (!File.Exists(filePath))
                return new List<LeaderboardEntry>();

            string json = await File.ReadAllTextAsync(filePath);

            return JsonSerializer.Deserialize<List<LeaderboardEntry>>(json)
                ?? new List<LeaderboardEntry>();
        }
        catch (JsonException)
        {
            return new List<LeaderboardEntry>();
        }
    }

    // TULEMUSE SALVESTAMINE
    public async Task SaveAsync(LeaderboardEntry entry)
    {
        List<LeaderboardEntry> entries = await LoadAsync();

        entries.Add(entry);

        // Edetabelisse jäävad 10 parimat tulemust.
        entries = entries
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.PlayedAt)
            .Take(10)
            .ToList();

        string json = JsonSerializer.Serialize(
            entries,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        Directory.CreateDirectory(
            Path.GetDirectoryName(filePath)!);

        await File.WriteAllTextAsync(filePath, json);
    }

    // EDETABELI NULLIMINE
    public Task ClearAsync()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
