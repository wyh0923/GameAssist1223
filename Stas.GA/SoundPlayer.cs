using System.Diagnostics;
using System.IO;
using NAudio.Wave;

namespace Stas.GA;
public class SoundPlayer {
    FileStream ba_stream;
    WaveStream ws;
    WaveOutEvent player;
    string full_name = null;
    float volume = 0.2f;
    int id = 0;// next play id
    int curr_id = 0;// current id
    DateTime play_until;
    public SoundPlayer() {
        var sp_thred = new Thread(() => {
            player ??= new WaveOutEvent();
            var w8 = 30; //tick w8ting in ms
            var pt = 0;//play time
            while (true) {
                if (full_name != null) {
                    if (play_until != DateTime.MinValue && play_until < DateTime.Now) {
                        full_name = null;
                        play_until = DateTime.MinValue;
                        continue; //w8 next
                    }
                    if (player.PlaybackState == PlaybackState.Playing) {
                        if (curr_id != id) {
                            player.Stop();
                            var st = 0; //stop time
                            while (player.PlaybackState == PlaybackState.Playing) {
                                Thread.Sleep(w8);
                                ui.AddToLog("PlaySound w8..[" + (st += w8) + "]");
                            }
                        }
                        Thread.Sleep(w8);
                        ui.AddToLog("Playinng a sound w8..[" + (pt += w8) + "]");
                        continue;
                    }

                    ba_stream = File.OpenRead(full_name);
                    Debug.Assert(full_name.EndsWith(".mp3"));
                    ws = new Mp3FileReader(ba_stream);
                    var wf = ws.WaveFormat;
                    if (wf.SampleRate != 44100 || wf.Channels != 1 || wf.BitsPerSample != 16) {
                        ws = new WaveFormatConversionStream(new WaveFormat(44100, 1), ws);
                    }
                    player.Init(ws);
                    player.Volume = volume;
                    curr_id = id;
                    play_until = DateTime.Now.AddMilliseconds(ws.TotalTime.TotalMilliseconds);
                    pt = 0;
                    player.Play();
                }
                Thread.Sleep(100);//cpu relax no play file in queue
            }
        });
        sp_thred.IsBackground = true;
        sp_thred.Start();
    }

    public void PlaySound(string fname, float _volume = 0.15f) {
        if (!ui.sett.b_can_play_sound) {
            if (ui.curr_role == Role.Slave) {
                ui.udp_sound.Send(Opcode.PlaySound, fname.To_UTF8_Byte());
            }
            return;
        }
        var _fn = Path.Combine(ui.sett.sounds_dir, fname);
        if (!File.Exists(_fn)) {
            ui.AddToLog(_fn + " NOT Found", MessType.Error);
            return;
        }
        id += 1;
        volume = _volume;
        full_name = _fn;
    }

    public void Dispose() {
        ws?.Close();
        ws?.Dispose();
        GC.SuppressFinalize(ws);
        ws = null;

        ba_stream?.Close();
        ba_stream?.Dispose();
        GC.SuppressFinalize(ba_stream);
        ba_stream = null;
    }
}