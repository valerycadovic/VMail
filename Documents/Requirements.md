# Требования к проекту
### Содержание
1. [Введение](#1)
2. [Требования пользователя](#2) <br>
  2.1. [Программные интерфейсы](#2.1) <br>
  2.2. [Интерфейс пользователя](#2.2) <br>
  2.3. [Характеристики пользователей](#2.3) <br>
3. [Системные требования](#3) <br>
  3.1 [Функциональные требования](#3.1) <br>
  3.2 [Нефункциональные требования](#3.2) <br>
    3.2.1 [Атрибуты качества](#3.2.1) <br>
      3.2.1.1 [Требования к удобству использования](#3.2.1.1) <br>
      3.2.1.2 [Требования к безопасности](#3.2.1.2) <br>
    3.2.2 [Ограничения](#3.2.2)
 4. [Аналоги](#4) <br>
  
### 1. Введение <a name="1"></a>
VMail - простое мобильное приложение для работы с электронной почтой. Позволяет просматривать и отправлять сообщения в любом формате данных, осуществлять их поиск по дате, вести адресную книгу и сохранять черновики. Предоставляет возможность работы как с несколькими аккаунтами на одном устройстве, так и с одним аккаунтом на нескольких без потери данных. Защита от спама гарантирует безопасность и порядок в почте.

### 2. Требования пользователя <a name="2"></a>
#### 2.1. Программные интерфейсы <a name="2.1"></a>
Проект использует фреймворк Xamarin и платформу для кроссплатформенной разработки приложений .NET Mono. 
#### 2.2. Интерфейс пользователя <a name="2.2"></a>
- Страница отправки сообщения.
Предлагает пользователю заполнить форму отправки сообщения и прикрепить вложенные файлы. <br>
  ![Message]( https://github.com/valerycadovic/VMail/blob/master/Mockups/Message.png)
- Страница списка входящих сообщений. 
На странице виден список сообщений. Каждое сообщение несёт в себе информацию о дате и времени отправления.<br>
  ![MessageList]( https://github.com/valerycadovic/VMail/blob/master/Mockups/MessagesList.png)
  
#### 2.3. Характеристики пользователей <a name="2.3"></a>
Приложение предназначено для пользователей любого возраста и любых социальных групп, желающих обмениваться информацией на расстоянии. Основной целевой аудиторией являются люди, имеющие потребность в отправке и получении быстрых уведомлений, касающейся работы и учёбы.
### 3. Системные требования <a name="3"></a>
Минимальная требуемая версия Android - 4.0.3.
#### 3.1. Функциональные требования <a name="3.1"></a>
Пользователю предоставлены возможности:
  1. Получение входящих сообщений.
  2. Чтение входящих сообщений.
  3. Отправка сообщений.

### 3.2 Нефункциональные требования <a name="3.2"></a>
* Наличие интернет-соединения.
* Наличие почтового ящика.

<a name="quality_attributes"/>

#### 3.2.1 Атрибуты качества <a name="3.2.1"></a>

<a name="requirements_for_ease_of_use"/>

##### 3.2.1.1 Требования к удобству использования <a name="3.2.1.1"></a>
1. Наличие меню для удобства навигации.
2. Указание времени получения и заголовка сообщения в списке.
3. Представление текста сообщения в формате HTML в виде веб-страницы.

<a name="security_requirements"/>

##### 3.2.1.2 Требования к безопасности <a name="3.2.1.2"></a>
1. Приложение предоставляет возможность доступа пользователей исключительно к тем почтовым ящикам, адрес и пароль которых им известны.
2. Неотправленные сообщения сохраняются в черновиках для последующей обработки.

### 3.2.2 Ограничения <a name="3.2.2"></a>
1. Мобильное приложение создаётся для платформы Android.
2. Целевая версия Android 8.0.
3. Минимальная версия Android 4.0.3.

### 4. Аналоги <a name="4"></a>
1. Приложение Gmail для Android(https://play.google.com/store/apps/details?id=com.google.android.gm&hl=ru)
Gmail мгновенно оповещает о новых письмах; в нем можно читать сообщения и писать ответы даже без подключения к Интернету, а также с легкостью выполнять поиск по всей почте.
2. Приложение Outlook для Windows(https://outlook.live.com/owa/)
Помимо функций почтового клиента для работы с электронной почтой, Microsoft Outlook является полноценным органайзером, предоставляющим функции календаря, планировщика задач, записной книжки и менеджера контактов.
 </br>
   VMail представляет собой классический почтовый клиент.
