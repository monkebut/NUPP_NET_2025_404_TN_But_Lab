# Лабораторна робота №2: Багатопоковість, Асинхронність, IEnumerables, LINQ

## Опис
Реалізація асинхронного thread-safe CRUD сервісу з підтримкою пагінації, серіалізації у файл та демонстрацією примітивів синхронізації.

## Структура проекту

- **CinemaManagement.Common/** - Бібліотека з моделями та CRUD сервісами
  - `CrudServiceAsync.cs` - Асинхронний thread-safe CRUD сервіс
  - `Movie.cs`, `Cartoon.cs`, `Ticket.cs`, `Customer.cs`, `Employee.cs` - Моделі з методами CreateNew()
  
- **CinemaManagement.Console/** - Консольний застосунок
  - `Program.cs` - Демонстрація роботи з паралельною генерацією та LINQ аналітикою

- **CinemaManagement.Tests/** - Юніт тести
  - `CrudServiceAsyncTests.cs` - 14 тестів для перевірки функціональності

## Запуск проекту

### Збірка проекту
```bash
dotnet build
```

### Запуск консольного застосунку
```bash
dotnet run --project CinemaManagement.Console/CinemaManagement.Console.csproj
```

### Запуск тестів
```bash
dotnet test CinemaManagement.Tests/CinemaManagement.Tests.csproj
```

## Реалізовані можливості

### 1. Асинхронний CRUD сервіс
- ✅ Інтерфейс `ICrudServiceAsync<T>` з методами: CreateAsync, ReadAsync, ReadAllAsync, UpdateAsync, RemoveAsync, SaveAsync
- ✅ Thread-safe реалізація на базі `ConcurrentDictionary<Guid, T>`
- ✅ Асинхронна серіалізація у JSON файли
- ✅ Підтримка пагінації через `ReadAllAsync(int page, int amount)`
- ✅ Реалізація `IEnumerable<T>` для підтримки LINQ

### 2. Багатопоточність
- ✅ Паралельна генерація 6200 об'єктів через `Parallel.For`
- ✅ Thread-safe операції з використанням `ConcurrentDictionary`
- ✅ Використання `SemaphoreSlim` для синхронізації файлових операцій

### 3. Примітиви синхронізації
- ✅ `lock` - взаємне виключення
- ✅ `SemaphoreSlim` - обмеження кількості одночасних потоків
- ✅ `AutoResetEvent` - сигналізація між потоками
- ✅ `ManualResetEvent` - множинне очікування

### 4. LINQ аналітика
- ✅ Мін/макс/середні значення для числових полів
- ✅ Групування та агрегація даних
- ✅ Фільтрація колекцій

### 5. Юніт тести (14 тестів)
- ✅ Тести для всіх CRUD операцій
- ✅ Тести пагінації
- ✅ Тести thread-safety
- ✅ Тести серіалізації
- ✅ Тести LINQ функціональності

## Результати виконання

Результати виконання програми збережені у файлі `Lab2_Results.txt`.

### Для конвертації у PDF:

#### Windows (PowerShell):
```powershell
# Використовуючи Microsoft Print to PDF
Get-Content Lab2_Results.txt | Out-Printer -Name "Microsoft Print to PDF"

# Або відкрити у Notepad та роздрукувати в PDF
notepad Lab2_Results.txt
```

#### Онлайн конвертація:
- https://www.online-convert.com/
- https://www.ilovepdf.com/txt_to_pdf
- https://convertio.co/txt-pdf/

#### Linux/Mac:
```bash
# Використовуючи pandoc
pandoc Lab2_Results.txt -o Lab2_Results.pdf

# Або enscript + ps2pdf
enscript Lab2_Results.txt -p - | ps2pdf - Lab2_Results.pdf
```

## Статистика генерованих даних

- Фільмів (Movies): 1500
- Мультфільмів (Cartoons): 1200
- Квитків (Tickets): 2000
- Клієнтів (Customers): 1000
- Співробітників (Employees): 500

**Загалом: 6200 об'єктів**

Час генерації: ~1 секунда

## Технічний стек

- .NET 9.0
- xUnit (тестування)
- System.Text.Json (серіалізація)
- System.Collections.Concurrent (thread-safe колекції)

## Автор

Студент групи 404_TN
Дата виконання: Грудень 2025

