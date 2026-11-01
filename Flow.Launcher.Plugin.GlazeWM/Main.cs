using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.GlazeWM
{
    public class Main : IPlugin
    {
        private PluginInitContext _context;

        private class CommandItem
        {
            public string Title { get; set; }
            public object Commands { get; set; }
        }

        private static readonly List<CommandItem> Items = new()
        {
            new() { Title = "toggle window management", Commands = new[] { "wm-toggle-pause" } },
            new() { Title = "focus the next active workspace", Commands = new[] { "focus", "--next-active-workspace" } },
            new() { Title = "focus the previous active workspace", Commands = new[] { "focus", "--prev-active-workspace" } },
            new() { Title = "resize mode", Commands = new[] { "wm-enable-binding-mode", "--name", "resize" } },
            new() { Title = "reload config", Commands = new[] { "wm-reload-config" } },
            new() { Title = "redraw", Commands = new[] { "wm-redraw" } },

            new() { Title = "workspace 1", Commands = new[] { "focus", "--workspace", "1" } },
            new() { Title = "workspace 2", Commands = new[] { "focus", "--workspace", "2" } },
            new() { Title = "workspace 3", Commands = new[] { "focus", "--workspace", "3" } },
            new() { Title = "workspace 4", Commands = new[] { "focus", "--workspace", "4" } },
            new() { Title = "workspace 5", Commands = new[] { "focus", "--workspace", "5" } },
            new() { Title = "workspace 6", Commands = new[] { "focus", "--workspace", "6" } },
            new() { Title = "workspace 7", Commands = new[] { "focus", "--workspace", "7" } },
            new() { Title = "workspace 8", Commands = new[] { "focus", "--workspace", "8" } },
            new() { Title = "workspace 9", Commands = new[] { "focus", "--workspace", "9" } },

            new() { Title = "move → workspace 1", Commands = new[] { new[] { "move", "--workspace", "1" }, new[] { "focus", "--workspace", "1" } } },
            new() { Title = "move → workspace 2", Commands = new[] { new[] { "move", "--workspace", "2" }, new[] { "focus", "--workspace", "2" } } },
            new() { Title = "move → workspace 3", Commands = new[] { new[] { "move", "--workspace", "3" }, new[] { "focus", "--workspace", "3" } } },
            new() { Title = "move → workspace 4", Commands = new[] { new[] { "move", "--workspace", "4" }, new[] { "focus", "--workspace", "4" } } },
            new() { Title = "move → workspace 5", Commands = new[] { new[] { "move", "--workspace", "5" }, new[] { "focus", "--workspace", "5" } } },
            new() { Title = "move → workspace 6", Commands = new[] { new[] { "move", "--workspace", "6" }, new[] { "focus", "--workspace", "6" } } },
            new() { Title = "move → workspace 7", Commands = new[] { new[] { "move", "--workspace", "7" }, new[] { "focus", "--workspace", "7" } } },
            new() { Title = "move → workspace 8", Commands = new[] { new[] { "move", "--workspace", "8" }, new[] { "focus", "--workspace", "8" } } },
            new() { Title = "move → workspace 9", Commands = new[] { new[] { "move", "--workspace", "9" }, new[] { "focus", "--workspace", "9" } } },

            new() { Title = "switch profile: no outer", Commands = new[] { "shell-exec", "--hide-window", "nu", @"%userprofile%/.glzr/glazewm/profiles/switch_no_outer.nu" } },
            new() { Title = "switch profile: with outer", Commands = new[] { "shell-exec", "--hide-window", "nu", @"%userprofile%/.glzr/glazewm/profiles/switch_with_outer.nu" } },
        };

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            var q = query.Search?.Trim() ?? "";
            var results = new List<Result>();

            foreach (var item in Items)
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    results.Add(CreateResult(item, null));
                    continue;
                }

                var match = _context.API.FuzzySearch(q, item.Title);
                if (match == null || !match.Success)
                    continue;

                results.Add(CreateResult(item, match));
            }

            return results.OrderByDescending(r => r.Score).ToList();
        }

        private Result CreateResult(CommandItem item, dynamic match)
        {
            return new Result
            {
                Title = item.Title,
                SubTitle = "",
                IcoPath = "assets/app.png",
                Score = match?.Score ?? 0,
                TitleHighlightData = match?.MatchData,
                Action = _ =>
                {
                    ExecuteCommands(item.Commands);
                    return true;
                }
            };
        }

        private void ExecuteCommands(object commands)
        {
            try
            {
                if (commands is string[] single)
                {
                    RunGlaze(single);
                }
                else if (commands is string[][] multi)
                {
                    foreach (var cmd in multi)
                        RunGlaze(cmd);
                }
            }
            catch (Exception ex)
            {
                _context.API.ShowMsg("GlazeWM Error", ex.Message);
            }
        }

        private void RunGlaze(string[] args)
        {
            var expandedArgs = args.Select(a => Environment.ExpandEnvironmentVariables(a)).ToArray();

            var startInfo = new ProcessStartInfo
            {
                FileName = "glazewm",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            startInfo.ArgumentList.Add("command");
            foreach (var arg in expandedArgs)
                startInfo.ArgumentList.Add(arg);

            using var process = Process.Start(startInfo);
            if (process == null) return;

            var error = process.StandardError.ReadToEnd();
            process.WaitForExit(3000);

            if (!string.IsNullOrWhiteSpace(error))
                _context.API.ShowMsg("GlazeWM", error.Trim());
        }
    }
}
