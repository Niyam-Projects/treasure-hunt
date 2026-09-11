# 🏴‍☠️ Developer Treasure Hunt

An interactive multi-protocol developer challenge written in ASP.NET Core (.NET 10). It spans REST APIs, GraphQL, and gRPC endpoints with dynamic, daily-rotating tokens.

---

## 🚀 Live Site Testing (`https://treasure-hunt.niyamit.com`)

### Automated Run
Execute all steps sequentially from your terminal:
```bash
bash test.sh
```

---

### Step-by-Step Manual Commands

#### **Step 1 — Initial Handshake (GET /)**
```bash
curl -i https://treasure-hunt.niyamit.com/
```
* Read the headers for `X-Next-Step` (`/api/v1/integrity-check`) and `X-Step-Token` (`INIT-XXXX`).

#### **Step 2 — Integrity Check (POST /api/v1/integrity-check)**
```bash
curl -i -X POST https://treasure-hunt.niyamit.com/api/v1/integrity-check \
  -H "X-Step-Token: <TOKEN_FROM_STEP_1>"
```
* Returns HTTP `418 I'm a teapot` with token `B-YYYY` and instructions to proceed to `/api/v1/reconcile`.

#### **Step 3 — Sequence Reconciliation (GET /api/v1/reconcile)**
```bash
curl -i https://treasure-hunt.niyamit.com/api/v1/reconcile \
  -H "X-Step-Token: <TOKEN_FROM_STEP_2>"
```
* Returns sequence `[101, 102, 103, 105, 106]` and token `SEQ-512`. Missing number is `104`.

#### **Step 4 — Core Completion (GET /api/v1/finish/104)**
```bash
curl -i https://treasure-hunt.niyamit.com/api/v1/finish/104 \
  -H "X-Step-Token: SEQ-512"
```
* Returns core challenge completion message and extra credit instructions + token `GQL-881`.

#### **Bonus Step 5 — GraphQL (POST /graphql)**

* **Introspection (Schema Discovery):**
  ```bash
  curl -X POST https://treasure-hunt.niyamit.com/graphql \
    -H "Content-Type: application/json" \
    -d '{"query":"{ __schema { queryType { fields { name description } } } }"}'
  ```

* **Unlock gRPC Phase:**
  ```bash
  curl -X POST https://treasure-hunt.niyamit.com/graphql \
    -H "Content-Type: application/json" \
    -d '{"query":"{ unlock(token: \"GQL-881\") { key nextStep instruction } }"}'
  ```
* Returns token `GRPC-ZZZZ` and instructions for gRPC completion.

#### **Bonus Step 6 — gRPC (`hunt.Hunt/Complete`)**

* **Service Discovery (Reflection):**
  ```bash
  grpcurl treasure-hunt.niyamit.com:443 list
  ```

* **Complete Final Challenge:**
  ```bash
  grpcurl -d '{"token": "<TOKEN_FROM_STEP_5>"}' treasure-hunt.niyamit.com:443 hunt.Hunt/Complete
  ```

---

## 💻 Local Development (`localhost`)

Run locally:
```bash
dotnet run
```
* **Port 8080:** HTTP/1.1 (REST & GraphQL)
* **Port 8586:** HTTP/2 (Direct gRPC)

### Local Commands:

```bash
# Step 1
curl -i http://localhost:8080/

# Step 2
curl -i -X POST http://localhost:8080/api/v1/integrity-check \
  -H "X-Step-Token: <TOKEN_FROM_STEP_1>"

# Step 3
curl -i http://localhost:8080/api/v1/reconcile \
  -H "X-Step-Token: <TOKEN_FROM_STEP_2>"

# Step 4
curl -i http://localhost:8080/api/v1/finish/104 \
  -H "X-Step-Token: SEQ-512"

# Bonus Step 5 (GraphQL)
curl -X POST http://localhost:8080/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"{ unlock(token: \"GQL-881\") { key nextStep instruction } }"}'

# Bonus Step 6 (gRPC Reflection & Call)
grpcurl -plaintext localhost:8586 list

grpcurl -plaintext \
  -d '{"token": "<TOKEN_FROM_STEP_5>"}' \
  localhost:8586 hunt.Hunt/Complete
```

---

## 📦 Deployment

Publish and package the release artifact:
```bash
dotnet publish TreasureHunt.csproj -c Release -o ./publish
cd publish && zip -r ../deploy.zip * && cd ..
```
Publish and deploy a .zip archive directly to Azure App Service (Linux).
