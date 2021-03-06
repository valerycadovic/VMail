# Содержание
1. [Отправка сообщения](#1)
2. [Чтение сообщения](#2)
3. [Авторизация](#3)

### 1. Отправка сообщения<a name="1"></a>
При загрузке страницы отправки сообщения отображается форма для заполнения данных о сообщении, включая получателей, тему, вложения и текст. Для создания и отправки нового сообщения необходимо заполнить эту форму, а затем нажать кнопку "Отправить". Если же форма заполнена не полностью, отобразится сообщение об ошибке и приложение потребует заполнить пустые поля. Иначе произойдет отправка сообщения. Если произошла ошибка при отправке, например сбой соединения с сервером или отправка на несуществующий адрес электронной почты, сообщение сохранится в черновики, после чего пользователь получит уведомление о невозможности выполнить отправку. Затем данные формы будут очищены и пользователь получит запрос на повторное её заполнение. В случае успешной отправки сообщения пользователь получит уведомление об успешности выполненной операции и откроется страница просмотра входящих сообщений.

![Отправка сообщения](https://github.com/valerycadovic/VMail/blob/master/Diagrams/Activity/Send.png)

### 2. Чтение сообщения<a name="2"></a>
На странице просмотра сообщений необходимо нажать на выбранное для прочтения сообщение в списке, после чего откроется страница сообщения. Если сообщение содержит какие-либо данные, они отобразятся. Иначе будет показано сообщение об ошибке чтения сообщения.

![Чтение сообщения](https://github.com/valerycadovic/VMail/blob/master/Diagrams/Activity/ViewMessage.png)

### 3. Авторизация<a name="3"></a>
После отображения приложением формы авторизации будет предложено заполнить форму, включающую в себя адрес электронной почты и пароль. Далее необходимо нажать на кнопку "Войти", после чего приложение проверит валидность введённых данных. Если произойдет ошибка аутентификации, будет выведено сообщение об ошибке аутентификации, форма будет очищена, а пользователю будет предложено повторить попытку. Если валидность введённых данных будет подтверждена, откроется страница входящих сообщений.

![Авторизация](https://github.com/valerycadovic/VMail/blob/master/Diagrams/Activity/Authorize.png)
