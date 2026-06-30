CRUD (создание, чтение, обновление, удаление) для трех таблиц:
1. **Users** (Пользователи)
2. **UserProfiles** (Профили, связь «один к одному» с Users)
3. **SystemConfigs** (Системные настройки, изолированная таблица)

---

1. Открыть `SimpleCrudApi.csproj` в IDE.
2. Запустить
3. Запустится в локальном порту (в терминале будет)

---

### 1. Пользователи (Users)
* **GET** `/users` — получить всех пользователей
* **GET** `/users/{id}` — получить пользователя по ID
* **POST** `/users` — создать пользователя
* **PUT** `/users/{id}` — обновить данные пользователя
* **DELETE** `/users/{id}` — удалить пользователя

### 2. Профили (UserProfiles)
* **GET** `/profiles` — получить все профили
* **GET** `/profiles/{id}` — получить профиль по ID
* **POST** `/profiles` — создать профиль для конкретного `UserId`
* **PUT** `/profiles/{id}` — обновить профиль
* **DELETE** `/profiles/{id}` — удалить профиль

### 3. Системные настройки (SystemConfigs)
* **GET** `/configs` — получить все настройки
* **GET** `/configs/{id}` — получить настройку по ID
* **POST** `/configs` — создать настройку
* **PUT** `/configs/{id}` — обновить настройку
* **DELETE** `/configs/{id}` — удалить настройку

---

### 1. Создание записи (POST)
```powershell
Invoke-RestMethod -Uri http://localhost:XXXX/users -Method Post -ContentType "application/json" -Body '{"username":"mops_test", "email":"mops@example.com"}'
```

### 2. Чтение списка (GET)
```powershell
Invoke-RestMethod -Uri http://localhost:XXXX/users -Method Get
```

### 3. Обновление записи (PUT)
```powershell
Invoke-RestMethod -Uri http://localhost:XXXX/users/1 -Method Put -ContentType "application/json" -Body '{"username":"mops_updated", "email":"new_mops@example.com"}'
```

### 4. Удаление записи (DELETE)
```powershell
Invoke-RestMethod -Uri http://localhost:XXXX/users/1 -Method Delete
```

### 5. Создание профиля для пользователя с ID = 1
```powershell
Invoke-RestMethod -Uri http://localhost:XXXX/profiles -Method Post -ContentType "application/json" -Body '{"userId":1, "fullName":"Иван Иванов", "bio":"Разработчик .NET"}'
```
