# Набор утилит
1. CleanerPhoneNumber - удаление номера телефона в строковых свойствах объекта с атрибутом NeedCleaningPhone
2. StringBuilderLimited - StringBuilder с ограничением кол-ва строк
3. ParallelPagesProcessor - параллельная обработка страниц данных
4. PeriodBatchesEnumerator - перебор данных с разбивкой по страницам за период с использованием LIMIT и OFFSET. Данный итератор стоит использовать, если в источнике данные распределены равномерно по дате и отсутствует сурогатный инкрементный ключ. В противном случае стоит рассмотреть решение https://habr.com/ru/companies/ruvds/articles/513766/
5. CreatorSqlScripts - генерация SQL скрипта создания таблиц из csv файла
