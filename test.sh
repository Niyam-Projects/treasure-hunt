#!/bin/bash

# Define the target site URL
BASE_URL="https://treasure-hunt.niyamit.com"

echo "--------------------------------------------------------"
echo "🏴‍☠️ Starting Treasure Hunt Verification Flow..."
echo "Target: $BASE_URL"
echo "--------------------------------------------------------"

# --- Step 1 ---
echo "1️⃣ Fetching welcome message and Step 1 token..."
HEADERS_1=$(curl -sD - -o /dev/null "$BASE_URL/")
TOKEN_1=$(echo "$HEADERS_1" | grep -i "x-step-token:" | awk '{print $2}' | tr -d '\r')
NEXT_STEP=$(echo "$HEADERS_1" | grep -i "x-next-step:" | awk '{print $2}' | tr -d '\r')

if [ -z "$TOKEN_1" ]; then
  echo "❌ Error: Failed to retrieve Step 1 token from headers."
  exit 1
fi
echo "✅ Step 1 Token: $TOKEN_1"
echo "✅ Next Step Path: $NEXT_STEP"
echo ""

# --- Step 2 ---
echo "2️⃣ Sending integrity check (POST)..."
RESPONSE_2=$(curl -s -X POST "$BASE_URL$NEXT_STEP" -H "X-Step-Token: $TOKEN_1")
TOKEN_2=$(echo "$RESPONSE_2" | sed -n 's/.*"key":"\([^"]*\)".*/\1/p')

if [ -z "$TOKEN_2" ]; then
  echo "❌ Error: Failed to retrieve Step 2 token. Response was: $RESPONSE_2"
  exit 1
fi
echo "✅ Step 2 Token: $TOKEN_2"
echo ""

# --- Step 3 ---
echo "3️⃣ Fetching sequence to reconcile..."
RESPONSE_3=$(curl -s "$BASE_URL/api/v1/reconcile" -H "X-Step-Token: $TOKEN_2")
TOKEN_3=$(echo "$RESPONSE_3" | sed -n 's/.*"key":"\([^"]*\)".*/\1/p')

if [ -z "$TOKEN_3" ]; then
  echo "❌ Error: Failed to retrieve Step 3 token. Response was: $RESPONSE_3"
  exit 1
fi
echo "✅ Step 3 Token: $TOKEN_3"
echo ""

# --- Step 4 ---
echo "4️⃣ Completing core challenge (sending missing number 104)..."
RESPONSE_4=$(curl -s "$BASE_URL/api/v1/finish/104" -H "X-Step-Token: $TOKEN_3")
echo "✅ Step 4 Completion Response:"
echo "$RESPONSE_4" | jq . 2>/dev/null || echo "$RESPONSE_4"
echo ""

# --- Step 5 ---
echo "5️⃣ Triggering GraphQL unlock for gRPC extra credit..."
RESPONSE_5=$(curl -s -X POST "$BASE_URL/graphql" \
  -H "Content-Type: application/json" \
  -d '{"query":"{ unlock(token: \"GQL-881\") { key nextStep instruction } }"}' )
echo "✅ GraphQL Step 5 Response:"
echo "$RESPONSE_5" | jq . 2>/dev/null || echo "$RESPONSE_5"
echo "--------------------------------------------------------"
echo "🎉 Verification flow completed successfully!"
echo "--------------------------------------------------------"
