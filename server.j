import express from 'express'
import cors from 'cors'
import { pool, poolConnect } from './db.js'

const app = express()
app.use(cors())
app.use(express.json())
poolConnect.catch(err => console.error(err))

app.get('/cita', async (req, res) => {
  try {
    const result = await pool.request().query(`
      SELECT c.id, c.fecha, c.motivo, c.tipo, c.estado,
             p.nombre, p.telefono
      FROM cita c
      JOIN pacientes p ON p.id = c.paciente_id
    `)
    res.json(result.recordset)
  } catch (err) {
    res.status(500).json(err)
  }
})

app.post('/cita', async (req, res) => {
  const { paciente, telefono, fecha, motivo, tipo, estado } = req.body
  try {
    const pacienteResult = await pool.request()
      .input('nombre', paciente)
      .input('telefono', telefono)
      .query(`INSERT INTO pacientes (nombre, telefono) OUTPUT INSERTED.ID VALUES (@nombre, @telefono)`)

    const pacienteId = pacienteResult.recordset[0].ID

    await pool.request()
      .input('pacienteid', pacienteId)
      .input('fecha', fecha)
      .input('motivo', motivo)
      .input('tipo', tipo)
      .input('estado', estado)
      .query(`INSERT INTO citas (pacienteid, fecha, motivo, tipo, estado) VALUES (@pacienteid, @fecha, @motivo, @tipo, @estado)`)

    await pool.request()
      .input('pacienteid', pacienteId)
      .input('fecha', fecha)
      .input('nota', motivo)
      .query(`INSERT INTO historial (pacienteid, fecha, nota) VALUES (@pacienteid, @fecha, @nota)`)

    res.json({ message: 'Cita creada con éxito' })
  } catch (err) {
    res.status(500).json(err)
  }
})

app.listen(process.env.PORT, () => console.log(`Backend corriendo en puerto ${process.env.PORT}`))