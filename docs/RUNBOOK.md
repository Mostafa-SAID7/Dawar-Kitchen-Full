# Production Runbook

## Pre-Deployment Checklist

- [ ] Database backup taken
- [ ] Environment variables prepared
- [ ] SSL/TLS certificates ready
- [ ] DNS records verified
- [ ] Team notified of deployment window

---

## Step 1 — Set Environment Variables

```bash
export PGHOST=prod-db-host.com
export PGPORT=5432
export PGUSER=prod_user
export PGPASSWORD=<secure-password>
export PGDATABASE=naar_noor_prod
export SUPABASE_URL=https://your-project.supabase.co
export SUPABASE_ANON_KEY=<anon-key>
export SUPABASE_SERVICE_ROLE_KEY=<service-role-key>
export JWT_SECRET_KEY=$(openssl rand -base64 32)
export ASPNETCORE_ENVIRONMENT=Production
```

## Step 2 — Build & Push Docker Images

```bash
docker-compose build
docker tag dawar-kitchen:backend  registry.example.com/dawar-kitchen:backend-v1.0
docker tag dawar-kitchen:frontend registry.example.com/dawar-kitchen:frontend-v1.0
docker push registry.example.com/dawar-kitchen:backend-v1.0
docker push registry.example.com/dawar-kitchen:frontend-v1.0
```

## Step 3 — Run Database Migrations

```bash
docker-compose exec dawar-kitchen-prod-api dotnet ef database update
```

## Step 4 — Start Services

```bash
docker-compose up -d
docker ps   # verify all containers running
```

## Step 5 — Verify Deployment

```bash
curl http://localhost/health
curl http://localhost:8080/health
curl http://localhost:8080/api/menu-items
docker logs dawar-kitchen-prod-api | tail -50
```

---

## Rollback

```bash
docker-compose down
docker tag registry.example.com/dawar-kitchen:backend-v0.9  dawar-kitchen:backend
docker tag registry.example.com/dawar-kitchen:frontend-v0.9 dawar-kitchen:frontend
docker-compose up -d
curl http://localhost/health
```

## Rollback Decision Table

| Symptom | Action |
|---------|--------|
| API not responding | Check `docker logs`, restart container |
| DB connection failed | Verify `PGHOST`, `PGPASSWORD` |
| High memory / OOM | Restart container, check for leaks |
| Frontend blank | Check Nginx config, CORS settings |
| Auth failures | Verify `JWT_SECRET_KEY` is set |
| Issues persist | Execute rollback above |

---

## Post-Deployment Checklist

- [ ] Frontend loads at dawar-kitchen.com
- [ ] API responds at /api/health
- [ ] DB migrations applied
- [ ] Login and JWT tokens work
- [ ] No errors in `docker logs`
- [ ] SSL certificate valid
