# 🚀 Деплой Ghost на Render.com

## Что такое Render?

**Render.com** — бесплатный хостинг для веб-приложений. Сервер будет работать 24/7 и будет доступен с **любого телефона** из любой точки мира.

---

## 📋 Шаг 1: Регистрация на Render

1. Зайди на **https://render.com**
2. Нажми **Sign Up**
3. Войди через **GitHub** (самый простой способ)
   - Если нет GitHub — создай на github.com

---

## 📋 Шаг 2: Push проекта на GitHub

### 2.1 Создай репозиторий на GitHub:
1. Зайди на **github.com**
2. Нажми **New Repository**
3. Название: `ghost-api`
4. Тип: **Public** (для бесплатного тарифа Render)
5. Нажми **Create repository**

### 2.2 Загрузи проект через Git:

Открой **PowerShell** в папке `C:\Users\user\Desktop\Ghost`:

```powershell
# Инициализация Git
git init

# Добавь все файлы
git add .

# Первый коммит
git commit -m "Initial commit: Ghost API + Mobile"

# Подключи репозиторий (ЗАМЕНИ ссылку на свою!)
git remote add origin https://github.com/ТВОЙ_НИК/ghost-api.git

# Отправь на GitHub
git branch -M main
git push -u origin main
```

---

## 📋 Шаг 3: Создание проекта на Render

### Вариант A: Автоматический (через render.yaml)

1. Зайди на **https://dashboard.render.com**
2. Нажми **Blueprints** → **Create Blueprint**
3. Выбери свой репозиторий `ghost-api`
4. Render **автоматически** создаст:
   - Веб-сервис (Ghost API)
   - PostgreSQL базу данных
5. Подожди 3-5 минут пока соберётся

### Вариант B: Ручной (если авто не сработал)

#### 3.1 Создай PostgreSQL базу:
1. **New** → **PostgreSQL**
2. Название: `ghost-db`
3. Region: **Frankfurt** (ближе к России)
4. Plan: **Free**
5. Нажми **Create**
6. **Скопируй Internal Database URL** (выглядит как `postgres://user:pass@host:5432/ghost`)

#### 3.2 Создай веб-сервис:
1. **New** → **Web Service**
2. Подключи репозиторий `ghost-api`
3. Настройки:
   - **Name**: `ghost-api`
   - **Region**: Frankfurt
   - **Branch**: main
   - **Root Directory**: `Ghost`
   - **Build Command**: `dotnet publish -c Release -o /app/publish`
   - **Start Command**: `dotnet /app/publish/Ghost.dll`
   - **Plan**: Free

4. **Environment Variables** (добавь вручную):

| Key | Value |
|-----|-------|
| `Db__Provider` | `postgres` |
| `DATABASE_URL` | *(вставь Internal Database URL из шага 3.1)* |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `Jwt__Key` | `GhostSuperSecretKeyForJWT2024!LongEnough32Chars` |
| `Jwt__Issuer` | `Ghost` |
| `Jwt__Audience` | `GhostUsers` |
| `Admin__SecretPhrase` | `24021983` |

5. Нажми **Create Web Service**

---

## 📋 Шаг 4: Проверка сервера

1. После деплоя Render покажет URL: `https://ghost-api-xxxx.onrender.com`
2. Открой в браузере: `https://ghost-api-xxxx.onrender.com/api/Tasks`
3. Должен вернуться `[]` (пустой массив) — значит сервер работает!

---

## 📋 Шаг 5: Обновление APK

### 5.1 Обновить URL сервера в приложении:

Путь: `Ghost.Mobile/Services/SettingsService.cs`

Найди строку:
```csharp
get => Preferences.Default.Get(ServerUrlKey, "http://10.0.2.2:5274");
```

Замени на:
```csharp
get => Preferences.Default.Get(ServerUrlKey, "https://ghost-api-xxxx.onrender.com");
```

*(Вставь свой URL вместо xxxxx)*

### 5.2 Пересобери APK:

```powershell
cd C:\Users\user\Desktop\Ghost\Ghost.Mobile
dotnet build -c Release
```

APK будет в: `bin/Release/net9.0-android/com.ghost.bazaar-Signed.apk`

### 5.3 Установи новый APK на телефон

---

## 📋 Шаг 6: Готово! 🎉

Теперь приложение доступно **всем** с любого устройства!

- Сервер: `https://ghost-api-xxxx.onrender.com`
- Админка: `https://ghost-api-xxxx.onrender.com/admin`
- APK: раздавай друзьям файл

---

## 🔧 Обновление кода

Когда внесёшь изменения в код:

```powershell
git add .
git commit -m "Описание изменений"
git push
```

Render **автоматически** пересоберёт и обновит сервер (~3-5 минут).

---

## ⚠️ Важно знать

### Free тариф Render:
- ✅ Бесплатно навсегда
- ⏸️ Засыпает после 15 мин без запросов (просыпается ~30 сек)
- 💾 750 часов/мес (хватает на 24/7 один сервис)
- 🗄️ База данных — 90 дней потом удаляется (для продакшена нужен платный)

### Для продакшена:
- Платный Render: $7/мес (Web Service) + $7/мес (PostgreSQL)
- Или VPS: Timeweb (~300₽/мес) — полный контроль

---

## 🐛 Troubleshooting

### Сервер не запускается:
```
Render Dashboard → Logs → смотри ошибки
```

### База данных не подключается:
```
Проверь DATABASE_URL в Environment Variables
```

### APK не подключается:
```
1. Проверь URL в SettingsService.cs
2. Убедись что сервер доступен в браузере
3. Пересобери APK
```

### 500 ошибка на сервере:
```
Render Dashboard → Logs → смотри стектрейс
```

---

## 📞 Если нужна помощь

Пиши — разберёмся! 👻
