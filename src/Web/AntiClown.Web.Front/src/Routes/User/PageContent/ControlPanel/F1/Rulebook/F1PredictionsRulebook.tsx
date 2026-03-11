import {
  Box,
  Card,
  CardContent,
  Chip,
  Divider,
  Link,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import React from "react";
import { Section } from "./Section";
import { SubSection } from "./SubSection";

const CURRENT_SEASON = new Date().getFullYear();

function Rule({ children }: { children: React.ReactNode }) {
  return (
    <Typography variant="body1" sx={{ pl: 1 }}>
      {"• "}
      {children}
    </Typography>
  );
}

function SubRule({ children }: { children: React.ReactNode }) {
  return (
    <Typography variant="body2" color="text.secondary" sx={{ pl: 3.5 }}>
      {"◦ "}
      {children}
    </Typography>
  );
}

function PointsTable({
  rows,
  col1,
  col2,
}: {
  rows: [string, string][];
  col1: string;
  col2: string;
}) {
  return (
    <Table size="small" sx={{ maxWidth: 420 }}>
      <TableHead>
        <TableRow>
          <TableCell sx={{ fontWeight: "bold" }}>{col1}</TableCell>
          <TableCell sx={{ fontWeight: "bold" }}>{col2}</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {rows.map(([label, points]) => (
          <TableRow key={label}>
            <TableCell>{label}</TableCell>
            <TableCell>{points}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}

export default function F1PredictionsRulebook() {
  return (
    <Stack spacing={6} sx={{ maxWidth: 860, pb: 6 }}>
      <Box>
        <Typography variant="h4" sx={{ fontWeight: "bold", mb: 1 }}>
          Регламент предсказаний F1
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Соревнование по предсказанию результатов гонок Формулы 1. Три
          независимых активности — предсказания на гонки, предсказания на
          чемпионат и бинго.
        </Typography>
      </Box>

      <Divider />

      <Section title="1. Предсказания на каждую гонку">
        <Typography variant="body1">
          Перед каждой гонкой необходимо заполнить несколько категорий
          предсказаний. Предсказания закрываются в момент старта гонки. Все
          категории независимы и начисляют очки отдельно. Карточки с{" "}
          <Chip
            label="красным"
            color="error"
            size="small"
            variant="outlined"
            sx={{ verticalAlign: "middle", mx: 0.3 }}
          />{" "}
          беджиком сезона не активны в {CURRENT_SEASON} году.
        </Typography>

        <SubSection
          title="10-е место"
          badge="до 25 очков"
          badgeColor="success"
          seasons={`2023-${CURRENT_SEASON}`}
          isActive={true}
        >
          <Rule>Выберите пилота, который финиширует на 10-й позиции.</Rule>
          <Rule>
            Очки зависят от того, насколько выбранный пилот отклонился от 10-го
            места в итоговом протоколе.
          </Rule>
          <Box sx={{ mt: 1 }}>
            <PointsTable
              col1="Итоговая позиция выбранного пилота"
              col2="Очки"
              rows={[
                ["10-е место (точно)", "25"],
                ["9-е или 11-е", "18"],
                ["8-е или 12-е", "15"],
                ["7-е или 13-е", "12"],
                ["6-е или 14-е", "10"],
                ["5-е или 15-е", "8"],
                ["4-е или 16-е", "6"],
                ["3-е или 17-е", "4"],
                ["2-е или 18-е", "2"],
                ["1-е, 19-е, 20-е", "1"],
                ["21-е, 22-е", "0"],
              ]}
            />
          </Box>
        </SubSection>

        <SubSection
          title="DNF (Не финишировавшие)"
          badge="до 10 очков"
          badgeColor="success"
          seasons={`2024-${CURRENT_SEASON}`}
          isActive={true}
        >
          <Rule>
            Выберите <strong>5 пилотов</strong>, которые сойдут с гонки, или
            отметьте «Никто не DNF».
          </Rule>
          <Rule>
            За каждого правильно угаданного DNF-пилота из 5 —{" "}
            <strong>2 очка</strong>.
          </Rule>
          <Rule>
            Если вы выбрали «Никто не DNF» и сходов не было —{" "}
            <strong>10 очков</strong>.
          </Rule>
          <Rule>
            Если вы выбрали «Никто не DNF», но хотя бы один пилот сошёл —{" "}
            <strong>0 очков</strong>.
          </Rule>
          <Rule>
            Повторения пилотов недопустимы — список должен содержать 5 разных
            пилотов.
          </Rule>

          <Divider sx={{ my: 0.5 }} />

          <Typography
            variant="body2"
            sx={{ pl: 1, fontWeight: "bold", color: "warning.main" }}
          >
            Важно: что считается DNF
          </Typography>
          <Rule>
            Засчитываются только пилоты, <strong>принявшие старт</strong> гонки
            и не завершившие её.
          </Rule>
          <SubRule>
            Пилот не выехал на прогревочный круг и не занял место на стартовой
            решётке — <strong>DNS</strong>, не DNF.
          </SubRule>
          <SubRule>
            Пилот проехал прогревочный круг, но не смог принять участие в старте
            гонки (например, заглох на решётке) — <strong>DNS</strong>, не DNF.
          </SubRule>
          <SubRule>
            Пилот был дисквалифицирован — неважно, во время гонки или после
            финиша — это <strong>DQ</strong>, не DNF.
          </SubRule>
          <SubRule>
            Пилот принял старт, но по любой причине не добрался до финиша —{" "}
            <strong>DNF</strong>.
          </SubRule>
          <Rule>
            В любых спорных случаях статус пилота определяется по официальному
            документу{" "}
            <Link href="#useful-links" underline="hover" color="inherit">
              <strong>Final Race Classification</strong>
            </Link>
            : если там указано <strong>DNF</strong> — засчитывается как DNF;
            любой другой статус (DNS, DQ и т.&thinsp;д.) — не засчитывается.
          </Rule>
        </SubSection>

        <SubSection
          title="Инциденты (Safety Car / VSC / Red Flag)"
          badge="5 очков"
          badgeColor="success"
          seasons={`2024-${CURRENT_SEASON}`}
          isActive={true}
        >
          <Rule>
            Выберите одно из четырёх значений: <strong>0</strong>,{" "}
            <strong>1</strong>, <strong>2</strong> или <strong>3+</strong>.
          </Rule>
          <Rule>
            Учитываются все инциденты гонки: машина безопасности (SC),
            виртуальная машина безопасности (VSC) и красные флаги.
          </Rule>
          <Rule>
            Если ваш ответ точно совпал с реальным количеством инцидентов —{" "}
            <strong>5 очков</strong>, иначе <strong>0</strong>.
          </Rule>
        </SubSection>

        <SubSection
          title="Отрыв лидера"
          badge="5 очков"
          badgeColor="success"
          seasons={`2024-${CURRENT_SEASON}`}
          isActive={true}
        >
          <Rule>
            Введите разрыв в секундах между 1-м и 2-м местом на финише
            (допускаются десятичные значения).
          </Rule>
          <Rule>
            Побеждает участник с <strong>наименьшим отклонением</strong> от
            реального отрыва — <strong>5 очков</strong>. Остальные получают 0.
          </Rule>
          <Rule>
            При одинаковом отклонении нескольких участников 5 очков получают
            все.
          </Rule>
        </SubSection>

        <SubSection
          title="Позиция гонщика"
          badge="до 10 очков"
          badgeColor="success"
          seasons={`2026`}
          isActive={true}
        >
          <Rule>
            Для каждой гонки определяется конкретный пилот. Ваша задача —
            предсказать его финишную позицию (от 1 до 22).
          </Rule>
          <Box sx={{ mt: 1 }}>
            <PointsTable
              col1="Отклонение от реальной позиции"
              col2="Очки"
              rows={[
                ["Точное попадание (±0)", "10"],
                ["±1 позиция", "7"],
                ["±2 позиции", "5"],
                ["±3 позиции", "3"],
                ["±4 позиции", "2"],
                ["±5 позиций", "1"],
                ["±6 и больше", "0"],
              ]}
            />
          </Box>
        </SubSection>

        <SubSection
          title="Команды (внутрикомандная дуэль)"
          badge="до 10 очков"
          badgeColor="success"
          seasons="2024–2025"
          isActive={CURRENT_SEASON >= 2024 && CURRENT_SEASON <= 2025}
        >
          <Rule>
            По каждой команде выберите пилота, который финиширует{" "}
            <strong>выше своего напарника</strong> в итоговом протоколе.
          </Rule>
          <Rule>
            За каждое верно угаданное командное противостояние начисляется 1
            очко.
          </Rule>
        </SubSection>

        <SubSection
          title="Спринт-гонки"
          badge="×0.30"
          badgeColor="warning"
          seasons="2025"
          isActive={CURRENT_SEASON === 2025}
        >
          <Rule>
            В 2025 году спринт-гонки засчитываются с коэффициентом{" "}
            <strong>×0.30</strong> от обычного количества очков.
          </Rule>
          <Rule>
            Обычные гонки в том же гоночном уик-энде засчитываются в полном
            объёме.
          </Rule>
        </SubSection>

        <Card
          sx={{ backgroundColor: "rgba(255,255,255,0.03)", borderRadius: 2 }}
        >
          <CardContent>
            <Typography variant="h6" sx={{ fontWeight: "bold", mb: 1.5 }}>
              Максимум очков за гонку
            </Typography>
            <PointsTable
              col1="Сезон"
              col2="Максимум за гонку"
              rows={[
                ["2023", "35 (10 место + DNF)"],
                [
                  "2024–2025",
                  "55 (10 место + DNF + Инциденты + Отрыв + Команды)",
                ],
                [
                  "2026",
                  "55 (10 место + DNF + Инциденты + Отрыв + Позиция гонщика)",
                ],
              ]}
            />
          </CardContent>
        </Card>
      </Section>

      <Divider />

      <Section title="2. Предсказания на чемпионат">
        <Typography variant="body1">
          Два раза за сезон вы составляете прогноз итоговой таблицы чемпионата
          пилотов. Предсказания делаются в два этапа, каждый из которых
          начисляет очки независимо.
        </Typography>

        <SubSection title="Этапы предсказаний">
          <Rule>
            <strong>Предсезонный</strong> — доступен до старта первой гонки.
            Расставьте всех пилотов сезона в порядке, в котором, по вашему
            мнению, они финишируют в чемпионате.
          </Rule>
          <Rule>
            <strong>Посреди сезона</strong> — доступен в период летнего
            перерыва. Можно скорректировать прогноз с учётом уже прошедших
            гонок.
          </Rule>
          <Rule>
            Для редактирования используется перетаскивание (drag &amp; drop)
            пилотов в списке.
          </Rule>
          <Rule>
            После закрытия каждого этапа изменить предсказания нельзя.
          </Rule>
          <Rule>
            Очки не начисляются каждую гонку! Они показывают, сколько очков вы
            бы получили, если бы сезон завершился прямо сейчас.
          </Rule>
        </SubSection>

        <SubSection
          title="Система очков за предсезонные предсказания"
          badge="до 20 очков за пилота"
          badgeColor="success"
        >
          <Rule>
            Очки за каждого пилота зависят от отклонения предсказанной позиции
            от реальной.
          </Rule>
          <Box sx={{ mt: 1 }}>
            <PointsTable
              col1="Отклонение"
              col2="Очки за пилота"
              rows={[
                ["Точно (±0)", "20"],
                ["±1 позиция", "14"],
                ["±2 позиции", "11"],
                ["±3 позиции", "8"],
                ["±4 позиции", "5"],
                ["±5 позиций", "2"],
                ["±6 и больше", "0"],
              ]}
            />
          </Box>
        </SubSection>

        <SubSection
          title="Система очков за предсказания посреди сезона"
          badge="до 10 очков за пилота"
          badgeColor="info"
        >
          <Rule>
            Та же логика, но с уменьшенными коэффициентами — ставки сделаны
            позже.
          </Rule>
          <Box sx={{ mt: 1 }}>
            <PointsTable
              col1="Отклонение"
              col2="Очки за пилота"
              rows={[
                ["Точно (±0)", "10"],
                ["±1 позиция", "6"],
                ["±2 позиции", "4"],
                ["±3 позиции", "2"],
                ["±4 позиции", "1"],
                ["±5 и больше", "0"],
              ]}
            />
          </Box>
        </SubSection>

        <SubSection title="Итоговый результат">
          <Rule>
            Итоговые очки за чемпионат ={" "}
            <strong>сумма предсезонных и посреди-сезонных очков</strong> по всем
            пилотам.
          </Rule>
          <Rule>
            На странице чемпионата отображается текущая таблица реального
            чемпионата рядом с вашими прогнозами, а также таблица лидеров среди
            всех участников.
          </Rule>
        </SubSection>
      </Section>

      <Divider />

      <Section title="3. Бинго">
        <Typography variant="body1">
          Бинго — коллекционная мини-игра в рамках сезона. Поле из 25 карточек с
          разными событиями Формулы 1. Карточки закрываются по мере того, как
          соответствующие события происходят в реальной жизни.
        </Typography>

        <SubSection title="Устройство поля">
          <Rule>
            Поле представляет собой сетку <strong>5×5</strong> из 25 карточек.
          </Rule>
          <Rule>
            Каждая карточка описывает определённое событие (например,{" "}
            <em>«Пилот сошёл на первом круге»</em>).
          </Rule>
          <Rule>
            Некоторые карточки имеют пояснение (иконка ℹ️) с уточнением условий
            засчитывания.
          </Rule>
          <Rule>
            Карточки могут срабатывать <strong>несколько раз</strong> за сезон
            (например, «Три SC за гонку» может произойти не один раз). Каждое
            срабатывание отображается небольшим кружком под карточкой.
          </Rule>
        </SubSection>

        <SubSection title="Вероятность событий">
          <Stack spacing={1}>
            <Stack direction="row" spacing={1} alignItems="center">
              <Box
                sx={{
                  width: 14,
                  height: 14,
                  borderRadius: 1,
                  backgroundColor: "darkred",
                  flexShrink: 0,
                }}
              />
              <Typography variant="body2">
                <strong>Низкая</strong> — редкое или маловероятное событие.
              </Typography>
            </Stack>
            <Stack direction="row" spacing={1} alignItems="center">
              <Box
                sx={{
                  width: 14,
                  height: 14,
                  borderRadius: 1,
                  backgroundColor: "#F4D06F",
                  flexShrink: 0,
                }}
              />
              <Typography variant="body2">
                <strong>Средняя</strong> — событие вполне реально, но не
                гарантировано.
              </Typography>
            </Stack>
            <Stack direction="row" spacing={1} alignItems="center">
              <Box
                sx={{
                  width: 14,
                  height: 14,
                  borderRadius: 1,
                  backgroundColor: "darkgreen",
                  flexShrink: 0,
                }}
              />
              <Typography variant="body2">
                <strong>Высокая</strong> — событие, которое, скорее всего,
                случится.
              </Typography>
            </Stack>
          </Stack>
          <Rule>Цвет рамки карточки соответствует вероятности события.</Rule>
          <Rule>
            Вероятность — исключительно информационный индикатор.{" "}
            <strong>Цвет карточки никак не влияет на подсчёт очков</strong>: все
            карточки, независимо от вероятности, засчитываются одинаково.
          </Rule>
        </SubSection>

        <SubSection title="Статус карточек">
          <Rule>
            Когда все повторения карточки выполнены, она считается{" "}
            <strong>завершённой</strong> и перечёркивается диагональной линией.
          </Rule>
          <Rule>
            Незавершённые карточки остаются активными до конца сезона.
          </Rule>
          <Rule>
            Бинго обновляется администраторами во время гонок по мере
            наступления событий.
          </Rule>
        </SubSection>
      </Section>

      <Divider />

      <Section title="Полезные ссылки" id="useful-links">
        <Card
          sx={{ backgroundColor: "rgba(255,255,255,0.05)", borderRadius: 2 }}
        >
          <CardContent>
            <Stack spacing={2}>
              <Stack spacing={0.5}>
                <Stack direction="row" spacing={2} alignItems="baseline">
                  <Typography variant="body1" sx={{ minWidth: 220 }}>
                    Официальные документы FIA
                  </Typography>
                  <Link
                    href="https://www.fia.com/documents/championships/fia-formula-one-world-championship-14"
                    target="_blank"
                    rel="noopener noreferrer"
                    underline="hover"
                  >
                    fia.com — FIA Formula One World Championship documents
                  </Link>
                </Stack>
                <Typography variant="body2" color="text.secondary">
                  Официальные документы чемпионата, в том числе{" "}
                  <strong>Final Race Classification</strong> — итоговые
                  протоколы каждой гонки.
                </Typography>
              </Stack>

              <Divider />

              <Stack spacing={0.5}>
                <Stack direction="row" spacing={2} alignItems="baseline">
                  <Typography variant="body1" sx={{ minWidth: 220 }}>
                    Liquipedia F1
                  </Typography>
                  <Link
                    href="https://liquipedia.net/formula1/Main_Page"
                    target="_blank"
                    rel="noopener noreferrer"
                    underline="hover"
                  >
                    liquipedia.net/formula1
                  </Link>
                </Stack>
                <Typography variant="body2" color="text.secondary">
                  Вики-энциклопедия по Формуле 1: расписание сезона, результаты
                  гонок, составы команд и история чемпионата.
                </Typography>
              </Stack>
            </Stack>
          </CardContent>
        </Card>
      </Section>
    </Stack>
  );
}
