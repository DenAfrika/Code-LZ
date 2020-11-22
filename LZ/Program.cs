using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace LZ
{
    public struct Code
    {
        public int position;
        public int length;
        public string symbol;
    }
    class Program
    {
        public static int BufferSize = 5;
        static void Main()
        {
            Console.WriteLine("Введите строку или нажмите Enter для просмотра тестов: ");
            string str = Console.ReadLine();
            if (str.Length > 0)
            {
                StartCode(str);
            }
            else
            {
                Console.WriteLine("Тест 1 'красная краска'");
                StartCode("красная краска");
                Console.WriteLine("Тест 2 'красивая краска'");
                StartCode("красивая краска");
                Console.WriteLine("Тест 3 'hello'");
                StartCode("hello");
            }
        }
        static void StartCode(string str)
        {
            str += " ";
            string dictionary = "";
            string buffer = "";
            List<Code> code = new List<Code>();
            int i = 0;
            int count = 0;
            while (i < str.Length)
            {
                if (str.Length - i < BufferSize)
                {
                    buffer = str.Substring(i, str.Length - i);
                }
                else
                    buffer = str.Substring(i, BufferSize);

                if (!dictionary.Contains(buffer.Substring(0, 1)))
                {
                    code.Add(new Code() { position = 0, length = 0, symbol = buffer.Substring(0, 1) });
                    dictionary += Convert.ToChar(buffer.Substring(0, 1));
                    i++;
                }
                else
                {
                    checkDictionary(dictionary, buffer, code);
                    i += code[count].length + 1;
                    if (i >= str.Length)
                        dictionary += buffer.Substring(0, code[count].length);
                    else
                        dictionary += buffer.Substring(0, code[count].length + 1);
                }
                count++;
            }
            for (int j = 0; j < count; j++)
            {
                Console.WriteLine("{0} {1} {2}", code[j].position, code[j].length, code[j].symbol);
            }
            Decode(code);
        }
        static void checkDictionary(string dictionary, string buffer, List<Code> code)
        {
            int i = 1;
            int j = 0;
            
            while (i <= buffer.Length)
                if (dictionary.Contains(buffer.Substring(0, i)))
                    i++;
                else
                    break;
            string str = dictionary.Substring(dictionary.Length - (i - 1), i - 1);
            str += str;
            while (buffer.Contains(str))
            {
                    str += dictionary.Substring(dictionary.Length - (i - 1), i - 1);
                    j += i - 1;
            }

            /*string str = buffer.Substring(0, i);
            int amount = new Regex(buffer.Substring(0, i - 1)).Matches(dictionary).Count;
            for(j = 0; j < amount; j++)
            {
                if (dictionary.Contains(str))
                {
                    str += buffer.Substring(0, i);
                    i += i; 
                }else
                    break;
            }*/
            code.Add(new Code() {position = dictionary.Length - dictionary.LastIndexOf(buffer.Substring(0, i - 1)), length = i - 1 + j, symbol = buffer.Substring(i - 1, 1) });
        }
        public static void Decode(List<Code> resList)
        {
            string res = "";
            int i = 0;
            foreach (var list in resList)
            {
                if (list.position == 0 && list.length == 0)
                {
                    res += list.symbol;
                }
                else
                {
                    if (list.position >= list.length)
                        res += res.Substring(res.Length - list.position, list.length) + list.symbol;
                    else
                    {
                        i = list.length + 1;
                        while (list.position < i)
                        {
                            res += res.Substring(res.Length - list.position, list.position);
                            i -= list.position;
                        }
                        res += list.symbol;
                    }
                }
            }
            Console.WriteLine(res);
        }
    }
}

/*
 * using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class LZ77
{
    /// <summary>
    /// Размер буфера
    /// </summary>
    public static int BufferSize = 5;
    /// <summary>
    /// Размер словаря (окна)
    /// </summary>
    public static int WindowSize = 14;

    public struct Node
    {
        /// <summary>
        /// Смещение назад
        /// </summary>
        public int Offset;
        /// <summary>
        /// Количество символов
        /// </summary>
        public int Length;
        /// <summary>
        /// Следующий символ
        /// </summary>
        public Char Next;
    }

    public static void Main()
    {
        Console.WriteLine("Введите строку или нажмите Enter для просмотра тестов: ");
        String s = Console.ReadLine();
        if (s.Length > 0)
        {
            var resList = Encode(s);

            // Вывод результата
            foreach (var res in resList)
            {
                Console.WriteLine("{0}\t{1}\t{2}", res.Offset, res.Length, res.Next);
            }
            Decode(resList);
        }
        else
        {
            Console.WriteLine("Тест 1 'красная краска'");
            var resList = Encode("красная краска");

            // Вывод результата
            foreach (var res in resList)
            {
                Console.WriteLine("{0}\t{1}\t{2}", res.Offset, res.Length, res.Next);
            }
            Decode(resList);
            Console.WriteLine("Тест 2 'красивая краска'");
            resList = Encode("красивая краска");

            // Вывод результата
            foreach (var res in resList)
            {
                Console.WriteLine("{0}\t{1}\t{2}", res.Offset, res.Length, res.Next);
            }
            Decode(resList);
            Console.WriteLine("Тест 3 'helloooooo'");
            resList = Encode("helloooooo");

            // Вывод результата
            foreach (var res in resList)
            {
                Console.WriteLine("{0}\t{1}\t{2}", res.Offset, res.Length, res.Next);
            }
            Decode(resList);
        }
        
    }

    public static List<Node> Encode(String s)
    {
        // Список - результат кодирования
        var resList = new List<Node>();

        // Начальная позиция
        int pos = 0;

        while (pos < s.Length)
        {
            int offset, length;
            // Поиск совпадения
            FindMatching(s, pos, out offset, out length);
            // Новая позиция
            pos += length + 1;
            if (pos <= s.Length) // Добавляем элемент результирующего списка
                resList.Add(new Node() { Offset = offset, Length = length, Next = s[pos - 1] });
            else // Добавляем элемент результирующего списка (100% последний, если для последнего символа/группы символов есть совпадение)
                resList.Add(new Node() { Offset = offset, Length = length, Next = new Char() });
        }

        return resList;
    }
    /// <summary>
    /// Поиск сопадения
    /// </summary>
    /// <param name="s">Кодируемое сообщение</param>
    /// <param name="pos">Позиция</param>
    /// <param name="offset">(выходной) размер сдвига назад</param>
    /// <param name="length">(выходной) количество символов</param>
    public static void FindMatching(String s, int pos, out int offset, out int length)
    {
        // Гарантированный результат при отсутствии совпадения
        offset = 0;
        length = 0;

        int endOfBuffer = Math.Min(pos + 1 + BufferSize, s.Length + 1);

        // Инициируем переменные для запоминания лучшего совпадения
        int bestMatchDistance = -1;
        int bestMatchLength = -1;

        // Цикл по буферу (от одного символа до размера буфера)
        for (int j = pos + 1; j < endOfBuffer; j++)
        {
            // Текущая подстрока из буфера
            String part = s.Substring(pos, j - pos);
            // Начальный индекс словаря в сообщении
            int startIndex = Math.Max(0, pos - WindowSize);

            // Цикл по словарю
            for (int i = startIndex; i < pos; i++)
            {
                // Количество необходимых повторений
                var repetitions = part.Length / (pos - i);
                // Необходимый остаток
                var last = part.Length % (pos - i);
                // Полученная проверяемая строка
                var matchedString = String.Concat(Enumerable.Repeat(s.Substring(i, pos - i), repetitions)) + s.Substring(i, last);

                if (matchedString == part && part.Length >= bestMatchLength)
                { // Совпадение случилось и оно длиннее и, возможно, ближе к концу
                    bestMatchDistance = pos - i;
                    bestMatchLength = part.Length;
                }
            }
        }

        if (bestMatchDistance > 0 && bestMatchLength > 0)
        { // Если было совпадение, то возвращаемые параметры обновятся
            offset = bestMatchDistance;
            length = bestMatchLength;
        }
    }
    public static void Decode(List<Node> resList)
    {
        string res = "";
        int i = 0;
        foreach (var list in resList)
        {
            if (list.Offset == 0 && list.Length == 0)
            {
                res += list.Next;
            }
            else
            {
                if (list.Offset >= list.Length)
                    res += res.Substring(res.Length - list.Offset, list.Length) + list.Next;
                else
                {
                    i = list.Length + 1;
                    while (list.Offset < i)
                    {
                        res += res.Substring(res.Length - list.Offset, list.Offset);
                        i -= list.Offset;
                    }
                    res += list.Next;
                }
            }
        }
        Console.WriteLine(res);
    }
}
*/
