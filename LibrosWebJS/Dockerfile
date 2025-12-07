FROM node:18-alpine

WORKDIR /app

# Copiar archivos de dependencias
COPY package.json .

# Instalar dependencias
RUN npm install

# Copiar el resto del código
COPY . .

# Exponer el puerto
EXPOSE 3000

# Comando de inicio
CMD ["node", "server.js"]