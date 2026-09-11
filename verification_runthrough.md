# 🏴‍☠️ Treasure Hunt Live Verification Runthrough

This document records the step-by-step verification of the live deployment at [https://treasure-hunt.niyamit.com](https://treasure-hunt.niyamit.com).

* **Timestamp:** `Mon, 31 Aug 2026 02:20:00 GMT` (UTC)
* **Status:** **PASS** (All 6 steps completed successfully)

---

## Step 1: Initial Handshake
Retrieves the initial welcome message and reads the dynamically generated UTC token from the response headers.

* **Request:**
  ```bash
  curl -sD - -o /dev/null https://treasure-hunt.niyamit.com/
  ```
* **Response Headers:**
  ```http
  HTTP/2 200 
  content-type: application/json
  date: Mon, 31 Aug 2026 02:20:00 GMT
  server: Kestrel
  x-next-step: /api/v1/integrity-check
  x-step-token: INIT-3204
  ```

---

## Step 2: Integrity Check
Submits the initial token to the integrity check endpoint via a `POST` request.

* **Request:**
  ```bash
  curl -s -X POST https://treasure-hunt.niyamit.com/api/v1/integrity-check \
    -H "X-Step-Token: INIT-3204"
  ```
* **Response Body:**
  ```json
  {
    "key": "B-1017",
    "instruction": "Reconcile the sequence at /api/v1/reconcile"
  }
  ```

---

## Step 3: Sequence Reconciliation
Fetches the sequence reconciliation task using the second token.

* **Request:**
  ```bash
  curl -s https://treasure-hunt.niyamit.com/api/v1/reconcile \
    -H "X-Step-Token: B-1017"
  ```
* **Response Body:**
  ```json
  {
    "sequence": [101, 102, 103, 105, 106],
    "task": "Find the missing integer 'x'. The final endpoint is /api/v1/finish/{x}",
    "key": "SEQ-512"
  }
  ```
  * *Calculation:* The missing number `x` is **`104`**.

---

## Step 4: Core Challenge Completion
Submits the missing number `104` to the finish endpoint.

* **Request:**
  ```bash
  curl -s https://treasure-hunt.niyamit.com/api/v1/finish/104 \
    -H "X-Step-Token: SEQ-512"
  ```
* **Response Body:**
  ```json
  {
    "success": true,
    "instructions": "You hunt very well! Email your resume to hr@niyamit.com with the subject: 'Challenge completed: INIT-3204-B-1017-SEQ-512-104-GQL-881'",
    "extraCredit": {
      "hint": "Two more steps remain for those who really know their protocols.",
      "graphql": "POST /graphql — run an introspection query to discover the schema.",
      "token": "GQL-881"
    }
  }
  ```

---

## Step 5: GraphQL Extra Credit
Calls the GraphQL mutation endpoint to unlock the gRPC stage credentials.

* **Request:**
  ```bash
  curl -s -X POST https://treasure-hunt.niyamit.com/graphql \
    -H "Content-Type: application/json" \
    -d '{"query":"{ unlock(token: \"GQL-881\") { key nextStep instruction } }"}'
  ```
* **Response Body:**
  ```json
  {
    "data": {
      "unlock": {
        "key": "GRPC-881",
        "nextStep": "treasure-hunt-two-czezcjajakenang5.eastus-01.azurewebsites.net:443 — call hunt.Hunt/Complete. Use reflection to discover the schema.",
        "instruction": "Pass your token in the 'token' field of CompleteRequest."
      }
    }
  }
  ```

---

## Step 6: gRPC Final Completion
Calls the live gRPC service on the main website using `grpcurl` (authenticated via reflection and dynamic token `GRPC-881`).

1. **Service Discovery Verification:**
   ```bash
   grpcurl treasure-hunt.niyamit.com:443 list
   ```
   *Output:*
   ```text
   grpc.reflection.v1alpha.ServerReflection
   hunt.Hunt
   ```

2. **gRPC Complete RPC Verification:**
   ```bash
   grpcurl -d '{"token": "GRPC-881"}' treasure-hunt.niyamit.com:443 hunt.Hunt/Complete
   ```
   *Output:*
   ```json
   {
     "success": true,
     "bonus_key": "BONUS-312",
     "instructions": "Great job! You completed the hunt! Email your resume to hr@niyamit.com with the subject: 'Challenge completed: INIT-3204-B-1017-SEQ-512-104-GQL-881-GRPC-881-BONUS-312'"
   }
   ```

---

## Summary
The restored gRPC service and the REST APIs now coexist on the production site. The application startup successfully avoided port collision issues on Azure Linux App Service and successfully loaded both protocols.
