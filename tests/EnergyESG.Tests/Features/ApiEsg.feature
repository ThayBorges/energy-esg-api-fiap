# language: pt
Funcionalidade: Contratos da API Energy ESG
  Como parte da governança de qualidade (ESG)
  Quero validar respostas HTTP e contratos JSON
  Para garantir rastreabilidade e conformidade das integrações

  Cenário: Listagem de unidades retorna documento paginado válido
    Quando solicito GET "/api/unidades"
    Então o status HTTP deve ser 200
    E o corpo deve validar o schema "paged-unidades.json"

  Cenário: Criação de unidade com nome inválido retorna erros de validação
    Quando envio POST "/api/unidades" com o JSON:
      """
      {"nome":"","endereco":"Rua X","tipo":"Empresarial"}
      """
    Então o status HTTP deve ser 400
    E o corpo deve validar o schema "fluent-validation-errors.json"

  Cenário: Relatório de energia com período inválido retorna erro de negócio
    Quando solicito GET "/api/relatorios/energia?unidadeId=00000000-0000-0000-0000-000000000001&de=2026-05-10&ate=2026-05-01"
    Então o status HTTP deve ser 400
    E o corpo deve validar o schema "texto-erro-api.json"

  Cenário: Listagem de sensores autenticada retorna array conforme contrato
    Quando solicito GET "/api/sensores"
    Então o status HTTP deve ser 200
    E o corpo deve validar o schema "sensor-list-item.json"

  Cenário: Consumo por período com datas inválidas rejeita a requisição
    Quando solicito GET "/api/relatorios/consumo-por-periodo?unidadeId=00000000-0000-0000-0000-000000000002&de=2026-06-01&ate=2026-05-01&agrupamento=dia"
    Então o status HTTP deve ser 400
    E o corpo deve validar o schema "texto-erro-api.json"
