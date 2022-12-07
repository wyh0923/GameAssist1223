using Coroutine;
using ImGuiNET;
using Coroutine;
using ImGuiNET;
using Stas.ImGuiNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using V2 = System.Numerics.Vector2;
using V4 = System.Numerics.Vector4;

namespace Stas.GA {
    public partial class DrawMain {
        readonly Dictionary<string, MovingAverage> MovingAverageValue = new();
        void DrawCorutine() {
            // The proc.PrivateMemorySize64 will returns the private memory usage in byte.
            // Would like to Convert it to Megabyte? divide it by 2^20
            ImGui.Text($"Total Used Memory: "+ui.game_watcher.memory_using + " (MB)");
            ImGui.Text($"Total Event Coroutines: {CoroutineHandler.EventCount}");
            ImGui.Text($"Total Tick Coroutines: {CoroutineHandler.TickingCount}");
            var cAI = ui.States.InGameStateObject.CurrentAreaInstance;
            ImGui.Text($"Total Entities: {cAI.AwakeEntities.Count}");
            ImGui.Text($"Currently Active Entities: {cAI.NetworkBubbleEntityCount}");
            var fps = ImGui.GetIO().Framerate;
            ImGui.Text($"FPS: {fps}");
            ImGui.NewLine();
            ImGui.Text($"==Average of last {(int)(1440 / fps)} seconds==");
            for (var i = 0; i < ui.CoroutinesRegistrar.Count; i++) {
                var coroutine = ui.CoroutinesRegistrar[i];
                if (coroutine.IsFinished) {
                        ui.CoroutinesRegistrar.Remove(coroutine);
                }

                if (MovingAverageValue.TryGetValue(coroutine.Name, out var value)) {
                    value.ComputeAverage(
                        coroutine.LastMoveNextTime.TotalMilliseconds,
                        coroutine.MoveNextCount);
                    ImGui.Text($"{coroutine.Name}: {value.Average:0.00}(ms)");
                }
                else {
                    MovingAverageValue[coroutine.Name] = new MovingAverage();
                }
            }
        }
        private class MovingAverage {
            readonly Queue<double> samples = new();
            readonly int windowSize = 144 * 10; // 10 seconds moving average @ 144 FPS.
            int lastIterationNumber;
            double sampleAccumulator;

            public double Average { get; private set; }

            /// <summary>
            ///     Computes a new windowed average each time a new sample arrives.
            /// </summary>
            /// <param name="newSample">new sample to add into the moving average.</param>
            /// <param name="iterationNumber">iteration number who's sample you are adding.</param>
            public void ComputeAverage(double newSample, int iterationNumber) {
                if (iterationNumber <= this.lastIterationNumber) {
                    return;
                }

                this.lastIterationNumber = iterationNumber;
                this.sampleAccumulator += newSample;
                this.samples.Enqueue(newSample);

                if (this.samples.Count > this.windowSize) {
                    this.sampleAccumulator -= this.samples.Dequeue();
                }

                this.Average = this.sampleAccumulator / this.samples.Count;
            }
        }

    }
    
}
