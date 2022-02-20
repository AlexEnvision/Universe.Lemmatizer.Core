//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Lemmatizer                                            ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Lemmatizer                                            ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Linq;
using System.Windows.Forms;
using Universe.Algorithm.MultiThreading;
using Universe.Algorithm.Tokenizer;
using Universe.Diagnostic.Logger;
using Universe.Helpers.Extensions;
using Universe.Lemmatizer.Tests.FormsApp.Adapters;
using Universe.Lemmatizer.Tests.FormsApp.Helpers;
using Universe.Lemmatizer.Tests.FormsApp.Settings;
using Universe.Windows.Forms.Controls;
using Universe.Windows.Forms.Controls.Settings;

namespace Universe.Lemmatizer.Tests.FormsApp
{
    public partial class MainForm : Form
    {
        private readonly EventLogger _log;

        private AppSettings _programSettings;

        private ThreadMachine _threadMachine;

        public MainForm()
        {
            InitializeComponent();

            _log = new EventLogger();

            _log.LogInfo += e =>
            {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] {e.Message}{Environment.NewLine}";
                    this.SafeCall(() => this.tbLog.AppendText(message));
                }
            };

            _log.LogError += e =>
            {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message =
                        $"[{currentDate}] Во время выполнения операции произошла ошибка. Текст ошибки: {e.Message}.{Environment.NewLine} Трассировка стека: {e.Ex.StackTrace}{Environment.NewLine}";
                    this.SafeCall(() => this.tbLog.AppendText(message));
                }
            };
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _programSettings = _programSettings.Load();
            LoadOnForm();
        }

        public void LoadOnForm()
        {
            tbInput.Text = _programSettings.Input;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnLoadFromForm();
            _programSettings.Save();
        }

        public void UnLoadFromForm()
        {
            _programSettings.Input = tbInput.Text;
        }

        private void btAnalyze_Click(object sender, EventArgs e)
        {
            var input = tbInput.Text;
            if (input.IsNullOrEmpty())
            {
                _log.Info("Не указаны входные данные. Укажите входные данные и попробуйте снова.");
                return;
            }

            btAnalyze.Enabled = false;

            _threadMachine = ThreadMachine.Create(1).RunInMultiThreadsWithoutWaiting(() => {
                try
                {
                    var textTokenizer = new TextTokenizer(input) { EmitTypes = Tokens.Word };
                    var inputTokens = textTokenizer.Tokenize().ToArray();

                    RusMorphologyAdapter adapter = new RusMorphologyAdapter();
                    var normaInputTokens = adapter.Lemmatizer.TransformMessage(inputTokens);

                    var normaText = string.Join(" ", normaInputTokens);

                    _log.Info($"Нормализованный текст: {normaText}.");
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);
                }
                finally
                {
                    this.SafeCall(() => btAnalyze.Enabled = true);
                }
            });
        }
    }
}