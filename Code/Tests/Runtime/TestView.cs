using System.Collections;
using Cli.Code.Runtime.Controller;
using Cli.Code.Runtime.Model;
using Cli.Code.Runtime.View;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Cli.Code.Tests.Runtime
{
    public class TestView
    {
        [SetUp]
        public void Setup()
        {
            SceneManager.LoadScene("Cli");
        }

        [UnityTest]
        public IEnumerator Messages()
        {
            yield return new WaitForEndOfFrame();
            var view = Object.FindObjectOfType<CliView>();
            Assert.NotNull(view);
            var cliService = new CliService();
            view.Init(cliService);
            view.ClearMessages();

            int size = 10;
            for (int i = 0; i < size; i++)
            {
                view.AddMessage($"hello {i}");
            }

            yield return new WaitForSeconds(1f);
            Assert.AreEqual(size, view.GetLines().Length);

            view.ClearMessages();
            yield return new WaitForEndOfFrame();

            Assert.Zero(view.GetLines().Length);
        }

        [UnityTest]
        public IEnumerator Input()
        {
            yield return new WaitForEndOfFrame();
            var view = Object.FindObjectOfType<CliView>();
            var cliService = new CliService();
            view.Init(cliService);

            yield return new WaitForSeconds(1f);
            Assert.NotNull(view);
            view.ClearMessages();
            view.Write("echo hello world");

            yield return new WaitForSeconds(1f);
            view.OnSubmit();

            yield return new WaitForSeconds(1f);
            Assert.True(string.IsNullOrEmpty(view.GetText()));
            Assert.GreaterOrEqual(view.GetLines().Length, 1);
        }

        [UnityTest]
        public IEnumerator History()
        {
            yield return new WaitForEndOfFrame();
            var view = Object.FindObjectOfType<CliView>();
            var cliService = new CliService();
            view.Init(cliService);

            Assert.NotNull(view);
            view.ClearMessages();

            var s1 = "foo";
            var s2 = "bar";
            var s3 = "hello";

            view.Write(s1);
            view.OnSubmit();
            yield return new WaitForEndOfFrame();
            view.Write(s2);
            view.OnSubmit();
            yield return new WaitForEndOfFrame();
            view.Write(s3);
            view.OnSubmit();
            yield return new WaitForEndOfFrame();

            view.PrevSuggestion();
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(view.GetText(), s3);

            view.PrevSuggestion();
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(view.GetText(), s2);

            view.PrevSuggestion();
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(view.GetText(), s1);

            view.PrevSuggestion();
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(view.GetText(), s3);

            view.NextSuggestion();
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(view.GetText(), s1);
        }

        [UnityTest]
        public IEnumerator AutoScroll()
        {
            yield return new WaitForEndOfFrame();
            var view = Object.FindObjectOfType<CliView>();
            var cliService = new CliService();
            view.Init(cliService);

            Assert.NotNull(view);
            view.ClearMessages();
            Assert.Zero(view.GetLines().Length);

            int size = 100;
            for (int i = 0; i < size; i++)
            {
                view.AddMessage($"foo {i}");
            }

            yield return new WaitForSeconds(1f);
            Assert.AreEqual(size, view.GetLines().Length);
            yield return new WaitForSeconds(1f);
            Assert.LessOrEqual(view.Scroll, 0.1f);

            view.Scroll = 0.5f;
            for (int i = 0; i < size; i++)
            {
                view.AddMessage($"bar {i})");
            }

            yield return new WaitForSeconds(1f);
            Assert.GreaterOrEqual(view.Scroll, 0.1f);

            view.ClearMessages();
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < size; i++)
            {
                view.AddMessage($"foo {i}");
            }

            yield return new WaitForSeconds(1f);
            Assert.AreEqual(size, view.GetLines().Length);
            yield return new WaitForSeconds(1f);
            Assert.LessOrEqual(view.Scroll, 0.1f);

            yield return new WaitForSeconds(1f);
        }

        [UnityTest]
        public IEnumerator SuggestionsSelect()
        {
            yield return new WaitForEndOfFrame();
            var view = Object.FindObjectOfType<CliView>();
            Assert.NotNull(view);

            var cliService = new CliService();
            view.Init(cliService);

            view.ClearMessages();

            int size = 20;

            for (int i = 0; i < size; i++)
            {
                cliService.RegisterCommand(new CommandInfo()
                {
                    Name = $"hello{i}",
                    Action = Foo
                });
            }

            view.Write("hello");
            view.PopulateSuggestions();
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < size; i++)
            {
                view.NextSuggestion();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void Foo(string[] obj)
        {
        }
    }
}