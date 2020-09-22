# ТЯП
- :white_check_mark: [Лексер](#Лексер)
- :white_check_mark: [Синтаксическй анализ](#Синтаксическй-анализ)
- :white_check_mark: [Таблица символов](#Таблица-символов)
- :white_check_mark: [Семантический анализ](#Семантический-анализ)
- :white_check_mark: [Кодогенератор](#Кодогенератор)

____
### Что требуется для компилирования
- .NET Core 2.1
- flat assembler [[гайд на установку](#Гайд-на-установку)]
- немного удачи
### Тесты
Для запуска тестов требуется:
- В корневой папке проекта зайти в папку lab1.
- Открыть командную строку и набрать
```
dotnet test
```
## Лексер
На вход программе принимается исходник с текстом. Текст парсится на допустимые лексемы и создает соответствующие токены.

Для запуска программы требуется:
- В корневой папке проекта зайти в папку lab1.
- Открыть командную строку и набрать
```
dotnet build
dotnet run --dump-tokens \source.cs
```
[:arrow_up:Оглавление](#ТЯП)
## Синтаксическй анализ
На выходе программа выдает AST с указанием ошибок

Для запуска программы требуется:
- В корневой папке проекта зайти в папку lab1.
- Открыть командную строку и набрать
```
dotnet build
dotnet run --dump-ast \source.cs
```
[:arrow_up:Оглавление](#ТЯП)
## Таблица символов
На вход AST, на выходе таблица символов с обозначеним вложенных областей и указание ошибок, что идентификатор повторно обьявляется

Для запуска программы требуется:
- В корневой папке проекта зайти в папку lab1.
- Открыть командную строку и набрать
```
dotnet build
dotnet run --dump-ast \source.cs
```

[:arrow_up:Оглавление](#ТЯП)
## Семантический анализ
На вход AST, на выходе анотированный AST

Для запуска программы требуется:
- В корневой папке проекта зайти в папку lab1.
- Открыть командную строку и набрать
```
dotnet build
dotnet run --dump-ast \source.cs
```

[:arrow_up:Оглавление](#ТЯП)
## Кодогенератор
На вход прнимается голова AST по которому алгоритм спускается и генерирует асемблер в файл с расширением .asm
Для запуска программы требуется:
- В корневой папке проекта зайти в папку lab1.
- Открыть командную строку и набрать
- C:\Fasm - это расположение корневой папки fasm компилятора, у вас может быть другое
```
dotnet build
dotnet run --dump-asm \source.cs C:\Fasm
```

[:arrow_up:Оглавление](#ТЯП)
## Гайд на установку
1) Переходим по этой [ссылке](https://flatassembler.net/download.php)
2) Скачиваем для той системы, которая стоит у вас (лучше на винду, я на ней только проверял :D)
3) Ставим (распаковываем) на диск, где стоит система (обязательно это или нет - я не знаю)
