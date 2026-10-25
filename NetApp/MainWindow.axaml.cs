using LocalAiApi.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace NetApp;

public partial class MainWindow : Window
{
    private HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:7110"),
        Timeout = TimeSpan.FromMinutes(5)
    };

    public bool isShow;
    public double baseWidth = 420;
    public double findWidth = 100;
    
    public MainWindow()
    {
        InitializeComponent();
    }
    
    public void ToogleButton_Click(object sender, RoutedEventArgs e)
    {
        isShow = !isShow;
        Width = isShow ? baseWidth : findWidth;
        ToggleButton.Content = isShow ? "<" : ">";
    }

    public async void GetButton_Click(object sender, RoutedEventArgs e)
    {
        GenerateButton.IsEnabled = false;
        ResponseTextBox.Text = "Анализ в процессе генерации. Ожидайте!";
        try
        {
            var request = new ChatRequest
            {
                Message = JsonSerializer.Serialize(new
                {
                    NoteTitle = "Название Заметки",
                    NoteContent = "рецепт блинов",
                    NoteType = "Личное"
                }),
                Prompt = "analyse"
            };
            using var response = await _httpClient.PostAsJsonAsync("api/chat", request);
            response.EnsureSuccessStatusCode();

            var finalAnswer = await response.Content.ReadAsStringAsync();
            ResponseTextBox.Text = finalAnswer ?? "ИИ не вернула ответ";
        }
        catch (Exception ex)
        {
            ResponseTextBox.Text = $"Не удалось получить анализ {ex.Message}";
        }
        finally
        {
            GenerateButton.IsEnabled = true;
        }
    }
    
}