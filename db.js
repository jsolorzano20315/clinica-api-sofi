import sql from 'mssql'
import dotenv from 'dotenv'

dotenv.config()

const dbConfig = {
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    server: process.env.DB_SERVER,
    database: process.env.DB_DATABASE,
    options: {
        encrypt: true,
        trustServerCertificate: true
    }
}

export const pool = new sql.ConnectionPool(dbConfig)
export const poolConnect = pool.connect()